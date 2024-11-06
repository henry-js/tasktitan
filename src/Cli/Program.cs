using Serilog;

using TaskTitan.Cli.AnsiConsole;
using TaskTitan.Cli.Commands;
using TaskTitan.Cli.Extensions;
using TaskTitan.Cli.Logging;
using TaskTitan.Core.Configuration;
using TaskTitan.Data;

using Tomlyn.Extensions.Configuration;

VelopackApp.Build()
    .WithFirstRun(v => { })
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
            config.AddTomlFile("reports.toml", false)
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
            config.AddTomlFile("udas.toml", true)
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
        })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton(_ => AnsiConsole.Console);
                services.AddSingleton(f => new LiteDbContext(LiteDbContext.CreateConnectionStringFrom(Global.DataDirectoryPath)));
                services.AddSingleton<IReportWriter, ReportWriter>();
                services.Configure<TaskTitanConfig>(_ =>
                {
                    context.Configuration.GetSection("Report").Bind(_.Report);
                    context.Configuration.GetSection("uda").Bind(_.Uda);
                });
            })
            .UseSerilog(SerilogConfig.LoggerConfiguration.CreateLogger())
            .UseProjectCommandHandlers()
            ;
    })
    .UseDefaults()
    .UseExceptionHandler((ex, context) => AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.NoStackTrace))
    .Build();

int result = await cmdLine.InvokeAsync(args);
return result;
