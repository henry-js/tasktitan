using LiteDB;

using TaskTitan.Data;
using TaskTitan.Lib.Parsing;

namespace tasktitan.Services;

[ServiceProvider]
[Singleton<ILoggerFactory>(Instance = nameof(LoggerFactory))]
[Singleton(typeof(ILogger<>), Factory = nameof(CreateLogger))]
[Import(typeof(IOptionsModule))]
[Transient<IConfigureOptions<CliConfig>>(Factory = nameof(BindCliConfig))]
[Singleton<TimeProvider>(Instance = nameof(SystemTimeProvider))]
[Singleton<ITaskService, TaskService>]
[Singleton<TaskCommands>]
[Singleton<IModificationParser, ParlotModificationParser>]
[Transient<IConfigureOptions<LiteDbOptions>>(Factory = nameof(BindLightDbOptions))]
[Singleton<LiteDatabase>(Factory = nameof(CreateLiteDb))]
[Singleton<LiteDbContext>]
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

    private TimeProvider SystemTimeProvider => TimeProvider.System;

    private static IConfigureOptions<CliConfig> BindCliConfig(IConfiguration configuration)
        => IOptionsModule
            .Configure<CliConfig>(config => configuration.Bind("Config", config));
    private static IConfigureOptions<LiteDbOptions> BindLightDbOptions()
    {
        return IOptionsModule.Configure<LiteDbOptions>(options => options.DatabaseDirectory = options.DatabaseDirectory);
    }
    private static LiteDatabase CreateLiteDb(IOptions<LiteDbOptions> options)
    {
        return new LiteDatabase(options.Value.ConnectionString);
    }

}
