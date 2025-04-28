using Velopack;

VelopackApp.Build().Run();

MyServiceProvider sp = new();

ConsoleApp.ServiceProvider = sp;
var app = ConsoleApp.Create();
app.Add<TaskCommands>();
app.UseFilter<ExceptionFilter>();

await app.RunAsync(args);