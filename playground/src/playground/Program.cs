
using Microsoft.Extensions.DependencyInjection;

using playground.DependencyInjection;
using playground.Filters;
using playground.Infrastructure.Data;

using Velopack;

VelopackApp.Build().Run();
var configuration = Extensions.CreateConfiguration();
var services = new ServiceCollection();
services.AddLogging(builder => builder.ConfigureSerilog());
services.AddSingleton(configuration);
services.AddSingleton<TaskTitanDbContext>();
services.AddSingleton<ITaskItemRepository, SqliteTaskItemRepository>();
ConsoleApp.ServiceProvider = services.BuildServiceProvider();

var app = ConsoleApp.Create();
app.Add<TaskCommands>();

app.UseFilter<ExceptionFilter>();
app.UseFilter<LogRunningTimeFilter>();

await app.RunAsync(args);
