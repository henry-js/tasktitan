using TaskTitan.Infrastructure.ExternalSync;
using TaskTitan.Infrastructure.ExternalSync.MicrosoftTodo;

namespace TaskTitan.Cli.Commands.Backup;

internal class ExportCommand : Command
{
    public ExportCommand() : base("export", "Export tasktitan tasks to a supported service")
    {
        AddOptions(this);
    }
    private static void AddOptions(Command command)
    {
        var allOption = new Option<bool>(aliases: ["-a", "--all"], () => false, "Whether to include deleted and completed tasks");
        command.AddOption(allOption);

        var serviceOption = new Option<SupportedService>(
            aliases: ["-s", "--service"], () => SupportedService.ToDo, "Service to use. Defaults to Microsoft To Do"
        );
        command.AddOption(serviceOption);

        var forceOption = new Option<bool>(aliases: ["-f", "--force"], () => true, "Force export without checking for updates");
        command.AddOption(forceOption);
    }
    new public class Handler(ILogger<ExportCommand> logger, IAnsiConsole console, IExternalTaskService service) : ICommandHandler
    {
        public SupportedService Service { get; set; }
        public bool All { get; set; }
        public bool Force { get; set; }

        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            logger.LogInformation("Fetching tasktitan tasks to export");

            var localTasks = await service.GetTasksToExportAsync(All);

            logger.LogInformation("Found {Count} tasks to export", localTasks.Count());
            bool listTasks = console.Confirm($"Found {localTasks.Count()} tasks to export. Display details?");

            if (listTasks)
                console.ListTasks(localTasks);

            string? listId = await service.GetListIdAsync();

            if (listId is null)
            {
                logger.LogInformation("No tasktitan export list in {Service}", Service);

                var createNewExportList = console.Confirm($"A tasktitan list could not be found in {Service}. Do you want to create now?");

                if (!createNewExportList)
                {
                    logger.LogInformation("Export cancelled");
                    return 0;
                }

                var result = await service.CreateNewListAsync();

                if (result.IsSuccess)
                    listId = result.Value;
                else
                {
                    foreach (var error in result.Errors)
                    {
                        console.WriteLine(error.Message);
                    }
                    console.WriteLine("Failed to create a list to export tasks to. Terminating program");
                    return -1;
                }
            }

            var exportedTasksResult = await service.FetchExistingExportedAsync(listId!);

            return exportedTasksResult.IsSuccess
                ? await PostTasksAsync(listId!, localTasks, exportedTasksResult.Value)
                : await PostTasksAsync(listId!, localTasks);
        }

        private async Task<int> PostTasksAsync(string listId, IEnumerable<TaskItem> tasksToExport, IEnumerable<Microsoft.Graph.Models.TodoTask>? fetchedTasks = null)
        {
            if (fetchedTasks is null || fetchedTasks.Count() == 0) Force = true;
            if (!Force)
            {
                var export = console.Confirm($"Export {tasksToExport.Count()} tasks to {Service}?");
                if (!export)
                {
                    console.WriteLine("Export cancelled by user");
                    return 0;
                }
                console.WriteLine("Check for existing tasks and implement export strategy");
                return 0;
            }
            await console.Progress()
                .StartAsync(async ctx =>
                {
                    var progressTask = ctx.AddTask($"[yellow]Exporting tasks...[/]");
                    int successes = 0;
                    int failures = 0;
                    double increment = 100 / tasksToExport.Count();

                    int i = 0;
                    foreach (var task in tasksToExport)
                    {
                        i++;
                        var result = await service.ExportTaskAsync(listId, task);
                        if (result.IsSuccess)
                        {
                            successes++;
                        }
                        else
                        {
                            console.MarkupLine("[red]Failed to export local task to cloud service[/]");
                            foreach (var error in result.Errors)
                            {
                                console.MarkupLineInterpolated($"\t[red]Reason: {error.Message}[/]");
                            }
                            failures++;
                        }
                        await Task.Delay(1000);
                        progressTask.Increment(increment);
                    }
                    console.WriteLine($"Export finished with {successes} successes and {failures} failures.");
                });

            return 0;
        }
    }
}
