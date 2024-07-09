﻿using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

using Bogus;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using TaskTitan.Cli.Commands.Actions;
using TaskTitan.Cli.Commands.Admin;
using TaskTitan.Cli.Commands.Backup;
using TaskTitan.Infrastructure;

using Velopack;

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Debug()
            .WriteTo.File("logs/startup_.log",
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
            rollingInterval: RollingInterval.Day
            )
            .Enrich.WithProperty("Application Name", "TaskTitan");
Log.Logger = loggerConfiguration.CreateBootstrapLogger();

VelopackApp.Build()
.WithFirstRun(v =>
{
    ConfigHelper.FirstRun();
    ConfigHelper.AddToPath();
})
#if WINDOWS // TODO: Make this cross-platform
.WithAfterUpdateFastCallback(v =>
{
    var initializer = new DatabaseInitializer($"Data Source={ConfigHelper.UserProfileDbPath}");
    initializer.InitializeAsync();
})
#endif
.Run();

// services.AddScoped<BogusCommand>();

var rootCommand = new RootCommand("task");
rootCommand.AddCommand(new ListCommand());
rootCommand.AddCommand(new AddCommand());
rootCommand.AddCommand(new StartCommand());
rootCommand.AddCommand(new ModifyCommand());
rootCommand.AddCommand(new DeleteCommand());
rootCommand.AddAdminCommands();
rootCommand.AddBackupCommands();

var cmdLineBuilder = new CommandLineBuilder(rootCommand);
Parser parser;
parser = cmdLineBuilder
    .UseHost(_ => Host.CreateDefaultBuilder(args), builder =>
    {
        builder.ConfigureServices(ConfigureServices)
        .UseCommandHandler<ListCommand, ListCommand.Handler>()
        .UseCommandHandler<AddCommand, AddCommand.Handler>()
        .UseCommandHandler<StartCommand, StartCommand.Handler>()
        .UseCommandHandler<ModifyCommand, ModifyCommand.Handler>()
        .UseCommandHandler<DeleteCommand, DeleteCommand.Handler>()
        .UseAdminCommandHandlers()
        .UseBackupCommandHandlers()
        .UseSerilog((context, services, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });
    })
    .UseDefaults()
    .UseExceptionHandler((ex, context) =>
    {
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenTypes | ExceptionFormats.NoStackTrace);
        Log.Fatal(ex, "Application terminated unexpectedly");
    }).Build();

int result = await parser.InvokeAsync(args);

#if DEBUG
Console.WriteLine($"Program terminated with code {result}");
Console.WriteLine("Press any key to exit.");
System.Console.ReadKey(intercept: true);
#endif

return result;

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddSingleton(_ => AnsiConsole.Console);
    services.AddSingleton(TimeProvider.System);
    services.AddInfrastructure();
    services.RegisterDb($"Data Source={ConfigHelper.UserProfileDbPath}", Log.Logger);

}
