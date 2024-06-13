using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using Spectre.Console.Cli.Extensions.DependencyInjection;

using TaskTitan.Cli.AdminCommands;
using TaskTitan.Cli.TaskCommands.Actions;
using TaskTitan.Lib.Dates;
using TaskTitan.Lib.Expressions;

using Velopack;

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Debug()
            .WriteTo.File("logs/application-.log", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .Enrich.WithProperty("Application Name", "TaskTitan");
Log.Logger = loggerConfiguration.CreateBootstrapLogger();

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

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false)
    .SetBasePath(Directory.GetCurrentDirectory())
    .Build();

var services = new ServiceCollection();
services.AddLogging(lbuilder =>
    lbuilder.AddSerilog(Log.Logger)
);

services.RegisterDb($"Data Source={ConfigHelper.UserProfileDbPath}", Log.Logger);

services.AddScoped<AddCommand>();
services.AddScoped<ListCommand>();
services.AddScoped<ModifyCommand>();
services.AddScoped<StartCommand>();
services.AddScoped<BogusCommand>();
services.AddScoped<ITaskItemService, TaskItemService>();
services.AddScoped<IStringFilterConverter<DateTime>, DateTimeConverter>();
services.AddScoped<IExpressionParser, ExpressionParser>();
services.AddSingleton(TimeProvider.System);

using var registrar = new DependencyInjectionRegistrar(services);
var app = new CommandApp<ListCommand>(registrar);

app.Configure(config =>
{
    config.PropagateExceptions();
    config.CaseSensitivity(CaseSensitivity.None);

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
});
try
{
    await app.RunAsync(args);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenTypes | ExceptionFormats.NoStackTrace);
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
