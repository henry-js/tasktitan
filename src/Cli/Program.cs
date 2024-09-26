using TaskTitan.Cli.Reports;

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
        ConfigHelper.EnsureTaskEnvVarExists();
        ConfigHelper.EnsureDirectoryExists();
        ConfigHelper.UpdateDatabase();
    })
.Run();


var userConfig = new ConfigurationBuilder()
    .AddTomlFile(ConfigHelper.UserConfigFile, true)
    .Build();

var reportConfig = new ConfigurationBuilder()
    .AddTomlFile("tasktitan.toml", false)
    .Build();
reportConfig.RemoveUnderscores();
TaskTitanOptions options = new();

reportConfig.Bind(options);

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
        builder.ConfigureAppConfiguration(c =>
        {
            c.AddTomlFile("tasktitan.toml", false)
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
        })

            .ConfigureServices(ConfigureServices)
        // builder.ConfigureHostConfiguration(c => c.AddTomlFile(ConfigHelper.UserConfigPath, true));
            .UseCommandHandler<ListCommand, ListCommand.Handler>()
            .UseCommandHandler<AddCommand, AddCommand.Handler>()
            .UseCommandHandler<StartCommand, StartCommand.Handler>()
            .UseCommandHandler<ModifyCommand, ModifyCommand.Handler>()
            .UseCommandHandler<DeleteCommand, DeleteCommand.Handler>()
            .UseAdminCommandHandlers()
            .UseBackupCommandHandlers()
            .UseSerilog((context, services, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));
    })
    .UseDefaults()
    // .UseExceptionHandler((ex, context) =>
    // {
    //     AnsiConsole.WriteException(ex, ExceptionFormats.Default);
    //     Log.Fatal(ex, "Application terminated unexpectedly");
    // })
    .Build();

int result = await parser.InvokeAsync(args);

#if DEBUG
AnsiConsole.WriteLine($"Program terminated with code {result}");
AnsiConsole.WriteLine("Press any key to exit.");
Console.ReadKey(intercept: true);
#endif

return result;

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    context.Configuration.RemoveUnderscores();
    services.Configure<Dictionary<ReportOptions.BuiltIn, ReportOptions>>(context.Configuration.GetSection("report"));
    services.AddSingleton(_ => AnsiConsole.Console);
    services.AddSingleton(TimeProvider.System);
    services.AddInfrastructure();
    services.RegisterDb(ConfigHelper.ConnectionString);
}
