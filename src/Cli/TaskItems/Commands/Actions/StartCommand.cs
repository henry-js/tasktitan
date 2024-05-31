using System.Threading.Tasks;

using Humanizer;

namespace TaskTitan.Cli.TaskItems.Commands.Actions;

internal sealed class StartCommand(IAnsiConsole console, ITaskItemService service, TaskTitanDbContext dbContext, ILogger<AddCommand> logger) : AsyncCommand<ActionSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, ActionSettings settings)
    {
        var tasksToStart = service.GetTasks();
        var tasktext = "task".ToQuantity(tasksToStart.Count());
        logger.LogInformation("Found {foundTasks} task(s)", tasksToStart.Count());
        foreach (var task in tasksToStart)
        {
            task.Start();
            service.Update(task);
        }

        dbContext.Commit();
        logger.LogInformation("Started {foundTasks} task(s)", tasksToStart.Count());

        var text = $"Updated " + tasktext;

        console.WriteLine(text);
        return Task.FromResult(0);
    }
}
