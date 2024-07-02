using System.CommandLine;
using System.CommandLine.Invocation;

using TaskTitan.Infrastructure.ExternalSync;
using TaskTitan.Infrastructure.ExternalSync.MicrosoftTodo;

namespace TaskTitan.Cli.Commands.Backup;

internal sealed class ImportCommand : Command
{
    public ImportCommand(Option<SupportedService> fromOption) : base("import", "Import existing tasks from supported service")
    {
        AddOption(fromOption);
        AddOptions(this);
    }
    private static void AddOptions(Command command)
    {
    }

    new public class Handler(IAnsiConsole console, ILogger<ImportCommand> logger, IExternalTaskService service)
    : ICommandHandler
    {
        public SupportedService ExternalTaskManager { get; set; }
        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            logger.LogInformation("Fetching tasks from {Service}", ExternalTaskManager);

            await console.Status()
                .StartAsync("Fetching...", async ctx =>
                {
                    var lists = await service.GetListsAsync();
                    console.WriteLine($"Retrieved {lists.Count()} lists");

                    ctx.Status("Retrieving tasks for each list");

                    foreach (var list in lists)
                    {
                        console.WriteLine($"Retrieving {list.DisplayName} tasks");
                        list.Tasks = (await service.GetTasks(list.Id)).ToList();
                    }
                });
            return 0;
        }
    }
}
