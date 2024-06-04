using Community.Extensions.Spectre.Cli.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using TaskTitan.Cli.AdminCommands;
using TaskTitan.Cli.TaskCommands;
using TaskTitan.Cli.TaskCommands.Actions;
using TaskTitan.Lib.Dates;
using TaskTitan.Lib.Expressions;
using TaskTitan.Lib.Text;

using Velopack;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/setup-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

// #if DEBUG
// ConfigHelper.FirstRun();
// ConfigHelper.AddToPath();
// #endif

VelopackApp.Build()
.WithFirstRun(v =>
{
    ConfigHelper.FirstRun();
    ConfigHelper.AddToPath();
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
    // builder.Services.RegisterDb(builder.Configuration, Log.Logger);

    // Add a command and optionally configure it.
    builder.Services.AddScoped<AddCommand>();
    builder.Services.AddScoped<ListCommand>();
    builder.Services.AddScoped<ModifyCommand>();
    builder.Services.AddScoped<StartCommand>();
    builder.Services.AddScoped<BogusCommand>();
    builder.Services.AddScoped<ITaskItemService, TaskItemService>();
    builder.Services.AddScoped<IDateTimeConverter, DateOnlyConverter>();
    builder.Services.AddScoped<IStringFilterConverter<DateTime>, DateTimeConverter>();
    builder.Services.AddScoped<IExpressionParser, ExpressionParser>();

    builder.Services.AddSingleton(TimeProvider.System);
    builder.Services.AddSingleton<ITextFilterParser, TextFilterParser>();
    // builder.Services.AddSingleton<DueDateHelper>();

    builder.UseSpectreConsole<ListCommand>(config =>
    {
        // All commands above are passed to config.AddCommand() by this point
        config.SetApplicationName("task");

        config.AddCommand<AddCommand>("add")
            .WithDescription("Add a task to the list");

        config.AddCommand<ListCommand>("list")
            .WithDescription("List tasks in default collection");

        config.AddCommand<ModifyCommand>("modify")
            .WithDescription("Modify an existing task");
        config.AddCommand<StartCommand>("start")
            .WithDescription("Start an existing task or create with description.");
        config.AddCommand<BogusCommand>("bogus")
            .WithDescription("Empty tasks table and fill with bogus data")
            .IsHidden();
        //         config.PropagateExceptions();
        // #if DEBUG
        //         config.UseBasicExceptionHandler();
        // #endif
        config.AddCommand<TestCommand>("test")
            .WithDescription("testing array argument vectors");
    });

    var app = builder.Build();


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

#if DEBUG
Console.WriteLine();
Console.WriteLine("Press any key to exit.");
System.Console.ReadKey(intercept: false);
#endif
