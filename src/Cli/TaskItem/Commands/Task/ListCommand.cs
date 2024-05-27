using System.Threading.Tasks;

using TaskTitan.Cli.Ttask.Models;

namespace TaskTitan.Cli.TaskItem.Commands;

internal sealed class ListCommand(IAnsiConsole console, ITtaskService service, ILogger<ListCommand> logger) : AsyncCommand<TaskSettings>
{
    private readonly IAnsiConsole console = console;
    private readonly ITtaskService service = service;
    private readonly ILogger<ListCommand> logger = logger;

    public override Task<int> ExecuteAsync(CommandContext context, TaskSettings settings)
    {
        if (settings.taskNum is null)
        {
            logger.LogDebug("Fetching all pending tasks");
            var pending = service.GetTasks().ToList().Select(t => TtaskDto.FromTtask(t));

            console.ListTasks(pending);
            return Task.FromResult(0);
        }
        logger.LogDebug("Fetching task {taskNum}", settings.taskNum);
        var task = service.Get(settings.taskNum.Value);
        if (task is not null)
        {
            console.DisplayTask(task);
            return Task.FromResult(0);
        }

        Console.WriteLine($"Task with taskNum {settings.taskNum} could not be found");
        return Task.FromResult(-1);
    }
}
