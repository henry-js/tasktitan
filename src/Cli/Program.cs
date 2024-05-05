using Community.Extensions.Spectre.Cli.Hosting;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

using Spectre.Console;
using Spectre.Console.Cli;

using TaskTitan.Cli;
using TaskTitan.Cli.Commands;
using TaskTitan.Cli.Commands.TaskCommands;
using TaskTitan.Data;
using TaskTitan.Lib;
using TaskTitan.Lib.Services;

using Velopack;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/setup-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    VelopackApp.Build()
    .WithFirstRun(v =>
    {
        if (!Directory.Exists(ConfigHelper.SourceDirectoryDataFolder))
        {
            throw new Exception(".tasktitan directory not found in source directory");
        }
        if (!Directory.Exists(ConfigHelper.UserProfileDirectoryDataFolder))
        {
            Directory.CreateDirectory(ConfigHelper.UserProfileDirectoryDataFolder);
        }
    })
    .Run();

    var configDir = ConfigHelper.FindTaskTitanDataFolder();

    var builder = Host.CreateApplicationBuilder(args);

    // Only use configuration in appsettings.json
    builder.Configuration.Sources.Clear();
    // builder.Configuration.AddJsonFile(Constants.AppSettingsPath, false);
    // Bind configuration section to object
    // builder.Services.AddOptions<NestedSettings>()
    //     .Bind(builder.Configuration.GetSection(NestedSettings.Key));

    //Disable logging
    builder.Logging.ClearProviders();
    builder.Services.AddSerilog((serv, lc) => lc
        .WriteTo.File(
            "logs/test-file-.log",
            rollingInterval: RollingInterval.Day,
            restrictedToMinimumLevel: LogEventLevel.Information
        )
        .WriteTo.Console(LogEventLevel.Information)
    );

#if DEBUG
    var path = Path.Combine(Directory.GetCurrentDirectory(), ".tasktitan", "tasks.db");
    builder.Services.RegisterDb($"Data Source={path}", Log.Logger);
#else
    builder.Services.RegisterDb($"Data Source={ConfigHelper.UserProfileDbPath}", Log.Logger);
#endif

    // Add a command and optionally configure it.
    builder.Services.AddScoped<AddCommand>();
    builder.Services.AddScoped<ISampleService, SampleService>(s => new SampleService("Other Service"));

    builder.UseSpectreConsole(config =>
    {
        // All commands above are passed to config.AddCommand() by this point
        config.SetApplicationName("task");

        config.AddCommand<AddCommand>("add")
            .WithDescription("Add a task to the list");

#if DEBUG
        config.UseBasicExceptionHandler();
#endif
    });

    var app = builder.Build();
    await using (var scope = app.Services.CreateAsyncScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<TaskTitanDbContext>();
        await db.Database.MigrateAsync();
    }
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

AnsiConsole.WriteLine("Press any key to exit.");
Console.ReadKey(intercept: false);
