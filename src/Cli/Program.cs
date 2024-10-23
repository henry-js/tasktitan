using System.Runtime.CompilerServices;
using TaskTitan.Cli.Commands;
using TaskTitan.Cli.Extensions;
using TaskTitan.Configuration;
using TaskTitan.Data;
using TaskTitan.Data.Reports;
using Tomlyn.Extensions.Configuration;

VelopackApp.Build()
    .WithFirstRun(v =>
    {
        Global.CreateConfigurationDirectories();
    })
    .Run();

var cmd = new RootCommand();
// cmd.AddGlobalOption(CliGlobalOptions.FilterOption);
cmd.AddCommand(new AddCommand());
cmd.AddCommand(new ListCommand());

var cmdLine = new CommandLineBuilder(cmd)
    .UseHost(_ => Host.CreateDefaultBuilder(args), builder =>
    {
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddJsonFile("appsettings.json", false)
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
            config.AddTomlFile("reports.toml", false)
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
        })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton(_ => AnsiConsole.Console);
                services.AddSingleton(f => new LiteDbContext(LiteDbContext.CreateConnectionStringFrom(Global.DataDirectoryPath)));
            })
            .UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration))
            .UseProjectCommandHandlers();
    })
    .UseDefaults()
    .Build();

int result = await cmdLine.InvokeAsync(args);

return result;
