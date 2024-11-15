using Serilog;

using TaskTitan.Cli.Commands;
using TaskTitan.Cli.Display;
using TaskTitan.Cli.Extensions;
using TaskTitan.Cli.Logging;
using TaskTitan.Core.Configuration;
using TaskTitan.Data;

using Tomlyn.Extensions.Configuration;

Log.Logger = SerilogConfig.LoggerConfiguration.CreateBootstrapLogger();
Log.Information("Application started");
VelopackApp.Build()
    .WithFirstRun(v => { })
    .Run();

var cmd = new RootCommand();
cmd.AddCommand(new AddCommand());
cmd.AddCommand(new ListCommand());
cmd.AddCommand(new NukeCommand());
cmd.AddCommand(new StartCommand());

var cmdLine = new CommandLineBuilder(cmd)
    .UseHost(_ => Host.CreateDefaultBuilder(args), builder =>
    {
        builder.UseConsoleLifetime()
            .UseSerilog(Log.Logger)
            .UseProjectCommandHandlers()
            .ConfigureAppConfiguration(config =>
            {
                config.AddTomlFile("reports.toml", false)
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                config.AddTomlFile("udas.toml", true)
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton(TimeProvider.System);
                services.AddSingleton(_ => AnsiConsole.Console);
                services.AddSingleton<LiteDbContext>();
                services.AddSingleton<ITaskActionHandler, TaskActionHandler>();
                services.Configure<TaskTitanConfig>(_ =>
                {
                    context.Configuration.GetSection("report").Bind(_.Report);
                    context.Configuration.GetSection("uda").Bind(_.Uda);
                });
                services.Configure<LiteDbOptions>(opts =>
                    opts.DatabaseDirectory = Global.DataDirectoryPath
                );
            });
    })
    .UseDefaults()
    .UseExceptionHandler((ex, context) =>
    {
        Log.Fatal(ex, "Fatal error encountered");
        // AnsiConsole.MarkupLineInterpolated($"[default on red]{ex.Message}[/]");
        AnsiConsole.WriteException(ex);
    })
    .Build();

Log.Information("Invoking commandline");
int result = await cmdLine.InvokeAsync(args);

#if DEBUG
Console.ReadLine();
#endif
return result;
