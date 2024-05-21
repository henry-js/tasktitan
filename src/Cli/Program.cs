using Community.Extensions.Spectre.Cli.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using TaskTitan.Cli.TaskCommands;
using TaskTitan.Lib.Dates;

using Velopack;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/setup-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

VelopackApp.Build()
.WithFirstRun(v =>
{
})
.Run();

try
{
        var builder = Host.CreateApplicationBuilder(args);

    // Bind configuration section to object
    // builder.Services.AddOptions<NestedSettings>()
    //     .Bind(builder.Configuration.GetSection(NestedSettings.Key));
    //Disable logging
    builder.Logging.ClearProviders();
    builder.Services.AddLogging(lbuilder =>
        lbuilder.AddSerilog(
            new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger())
    );

    builder.Services.RegisterDb($"Data Source={ConfigHelper.UserProfileDbPath}", Log.Logger);

    // Add a command and optionally configure it.
    builder.Services.AddScoped<AddCommand>();
    builder.Services.AddScoped<ListCommand>();
    builder.Services.AddScoped<ModifyCommand>();
    builder.Services.AddScoped<ITtaskService, TaskService>();
    builder.Services.AddScoped<IDateTimeConverter, DateOnlyConverter>();
    builder.Services.AddSingleton(TimeProvider.System);
    // builder.Services.AddSingleton<DueDateHelper>();

    builder.UseSpectreConsole(config =>
    {
        // All commands above are passed to config.AddCommand() by this point
        config.SetApplicationName("task");

        config.AddCommand<AddCommand>("add")
            .WithDescription("Add a task to the list");

        config.AddCommand<ListCommand>("list")
            .WithDescription("List tasks in default collection");

        config.AddCommand<ModifyCommand>("modify")
            .WithDescription("Modify an existing task");
        //         config.PropagateExceptions();
        // #if DEBUG
        //         config.UseBasicExceptionHandler();
        // #endif
    });

    var app = builder.Build();

    ConfigHelper.FirstRun();

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

Console.WriteLine();
Console.WriteLine("Press any key to exit.");
System.Console.ReadKey(intercept: false);
