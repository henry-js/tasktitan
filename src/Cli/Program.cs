using Community.Extensions.Spectre.Cli.Hosting;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

using Spectre.Console.Cli;

using TaskTitan.Cli;
using TaskTitan.Cli.Commands;
using TaskTitan.Data;
using TaskTitan.Lib;
using TaskTitan.Lib.Services;

using Velopack;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/setup.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    VelopackApp.Build().Run();

    var builder = Host.CreateApplicationBuilder(args);

    // Only use configuration in appsettings.json

    builder.Configuration.Sources.Clear();
    builder.Configuration.AddJsonFile(Constants.AppSettingsPath, false);

    //Disable logging
    builder.Logging.ClearProviders();

    builder.Services.RegisterDb(Constants.DbConnectionString);

    // Bind configuration section to object
    builder.Services.AddOptions<NestedSettings>()
        .Bind(builder.Configuration.GetSection(NestedSettings.Key));

    // Add a command and optionally configure it.
    builder.Services.AddCommand<HelloCommand>("hello", cmd =>
    {
        cmd.WithDescription("A command that says hello");
    });

    // Add another command and its dependent service

    builder.Services.AddCommand<OtherCommand>("other");
    builder.Services.AddScoped<ISampleService, SampleService>(s => new SampleService("Other Service"));

    //
    // The standard call save for the commands will be pre-added & configured
    //
    builder.UseSpectreConsole<HelloCommand>(configureCommandApp =>
    {
        // All commands above are passed to config.AddCommand() by this point
        configureCommandApp.SetApplicationName("hello");
        configureCommandApp.UseBasicExceptionHandler();
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

Console.ReadKey(intercept: false);
