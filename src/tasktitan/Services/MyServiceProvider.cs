namespace tasktitan.Services;

[ServiceProvider]
[Singleton<ILoggerFactory>(Instance = nameof(LoggerFactory))]
[Singleton(typeof(ILogger<>), Factory = nameof(CreateLogger))]
[Import(typeof(IOptionsModule))]
[Transient<IConfigureOptions<CliConfig>>(Factory = nameof(BindCliConfig))]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
[Singleton<MyCommands>]
[Singleton<IConfiguration>(Factory = nameof(CreateConfiguration))]

internal partial class MyServiceProvider
{
    private IConfiguration CreateConfiguration()
        => new ConfigurationBuilder()
            .AddJsonFile("./config.json", false)
            .Build();

    private ILoggerFactory LoggerFactory
        => MSLogger.Create(builder => builder.AddSerilog(
            new LoggerConfiguration()
                    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "application.log"),
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
                        rollingInterval: RollingInterval.Day,
                        shared: true)
                    .Enrich.WithProperty("Application Name", "<APP NAME>")
                    .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
                .CreateLogger()));

    private ILogger<T> CreateLogger<T>()
        => LoggerFactory.CreateLogger<T>();

    private static IConfigureOptions<CliConfig> BindCliConfig(IConfiguration configuration)
        => IOptionsModule
            .Configure<CliConfig>(config => configuration.Bind("Config", config));
}
