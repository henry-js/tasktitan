using System.Threading.Tasks;

using Humanizer;

using TaskTitan.Core.Queries;
using TaskTitan.Lib.Expressions;

namespace TaskTitan.Cli.TaskCommands.Actions;

internal sealed class StartCommand(IAnsiConsole console, ITaskItemService service, ILogger<AddCommand> logger) : AsyncCommand<StartCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, StartCommandSettings settings)
    {
        IEnumerable<Expression> filters = [];
        var tasksToStart = await service.GetTasks(settings.filterText ?? []);
        var tasktext = "task".ToQuantity(tasksToStart.Count());
        logger.LogInformation("Found {foundTasks} task(s)", tasksToStart.Count());
        foreach (var task in tasksToStart)
        {
            task.Start();
            await service.Update(task);
        }

        logger.LogInformation("Started {foundTasks} task(s)", tasksToStart.Count());

        var text = $"Updated " + tasktext;

        console.WriteLine(text);
        return 0;
    }
}
