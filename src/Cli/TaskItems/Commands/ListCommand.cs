using System.Threading.Tasks;

using TaskTitan.Cli.TaskItems.Models;

namespace TaskTitan.Cli.TaskItems.Commands;

internal sealed class ListCommand(IAnsiConsole console, ITaskItemService service, ILogger<ListCommand> logger) : AsyncCommand<TaskSettings>
{
    private readonly IAnsiConsole console = console;
    private readonly ITaskItemService service = service;
    private readonly ILogger<ListCommand> logger = logger;

    public override Task<int> ExecuteAsync(CommandContext context, TaskSettings settings)
    {
        if (settings.taskNum is null)
        {
            logger.LogDebug("Fetching all pending tasks");
            var pending = service.GetTasks().ToList().Select(t => TaskItemDto.FromTaskItem(t));

            console.ListTasks(pending);
            return System.Threading.Tasks.Task.FromResult(0);
        }
        logger.LogDebug("Fetching task {taskNum}", settings.taskNum);
        var task = service.Get(settings.taskNum.Value);
        if (task is not null)
        {
            console.DisplayTask(task);
            return System.Threading.Tasks.Task.FromResult(0);
        }

        Console.WriteLine($"Task with taskNum {settings.taskNum} could not be found");
        return System.Threading.Tasks.Task.FromResult(-1);
    }
}
