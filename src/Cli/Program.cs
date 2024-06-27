using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using TaskTitan.Cli.AdminCommands;
using TaskTitan.Cli.TaskCommands.Actions;
using TaskTitan.Lib.Dates;
using TaskTitan.Lib.Expressions;

using Velopack;

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Debug()
            .WriteTo.File("logs/startup_.log", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u}] {SourceContext}: {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day)
            .Enrich.WithProperty("Application Name", "TaskTitan");
Log.Logger = loggerConfiguration.CreateBootstrapLogger();

VelopackApp.Build()
.WithFirstRun(v =>
{
    ConfigHelper.FirstRun();
    ConfigHelper.AddToPath();
}).Run();

// services.AddScoped<BogusCommand>();

var rootCommand = new RootCommand("task");
rootCommand.AddCommand(new ListCommand());
rootCommand.AddCommand(new AddCommand());
rootCommand.AddCommand(new StartCommand());
rootCommand.AddCommand(new ModifyCommand());
rootCommand.AddCommand(new BogusCommand());

var cmdLineBuilder = new CommandLineBuilder(rootCommand);
int result = 0;

var parser = cmdLineBuilder
    .UseHost(_ => Host.CreateDefaultBuilder(args), builder =>
    {
        builder.ConfigureServices(ConfigureServices)
        .UseCommandHandler<ListCommand, ListCommand.Handler>()
        .UseCommandHandler<AddCommand, AddCommand.Handler>()
        .UseCommandHandler<ModifyCommand, ModifyCommand.Handler>()
        .UseCommandHandler<StartCommand, StartCommand.Handler>()
        .UseCommandHandler<BogusCommand, BogusCommand.Handler>();

        builder.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));
    })
    .UseDefaults()
    .UseExceptionHandler((ex, context) =>
    {
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenTypes | ExceptionFormats.NoStackTrace);
        Log.Fatal(ex, "Application terminated unexpectedly");
    }).Build();

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddSingleton(_ => AnsiConsole.Console);
    services.AddScoped<ITaskItemService, TaskItemService>();
    services.AddScoped<IStringFilterConverter<DateTime>, DateTimeConverter>();
    services.AddScoped<IExpressionParser, ExpressionParser>();
    services.AddSingleton(TimeProvider.System);
    services.RegisterDb($"Data Source={ConfigHelper.UserProfileDbPath}", Log.Logger);
}
result = await parser.InvokeAsync(args);

// config.AddCommand<BogusCommand>("bogus")
//     .WithDescription("Empty tasks table and fill with bogus data")
//     .IsHidden();

#if DEBUG
Console.WriteLine();
Console.WriteLine("Press any key to exit.");
System.Console.ReadKey(intercept: false);
#endif

return result;
