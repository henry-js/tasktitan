using System.CommandLine;
using System.CommandLine.Invocation;

using TaskTitan.Infrastructure.ExternalSync;
using TaskTitan.Infrastructure.ExternalSync.MicrosoftTodo;

namespace TaskTitan.Cli.Commands.Backup;

internal sealed class ImportCommand : Command
{
    public ImportCommand(Option<SupportedService> serviceOption) : base("import", "Import existing tasks from supported service")
    {
        AddOption(serviceOption);
        AddOptions(this);
    }
    private static void AddOptions(Command command)
    {
    }

    new public class Handler(IAnsiConsole console, ILogger<ImportCommand> logger, IExternalTaskService service)
    : ICommandHandler
    {
        public SupportedService Service { get; set; }
        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            logger.LogInformation("Fetching tasks from {Service}", Service);

            await console.Status()
                .StartAsync("Fetching...", (Func<StatusContext, Task>)(async ctx =>
                {
                    var lists = await service.GetListsAsync();
                    console.WriteLine($"Retrieved {lists.Count()} lists");

                    ctx.Status("Retrieving tasks for each list");

                    foreach (var list in lists)
                    {
                        console.WriteLine($"Retrieving {list.DisplayName} tasks");
                        var result = await service.FetchExistingExportedAsync(list.Id);
                        if (result.IsSuccess)
                            list.Tasks = result.Value?.ToList();
                    }
                }));
            return 0;
        }
    }
}
