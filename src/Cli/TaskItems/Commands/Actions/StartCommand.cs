using System.Threading.Tasks;

using Humanizer;

using TaskTitan.Core.Queries;
using TaskTitan.Lib.Text;

namespace TaskTitan.Cli.TaskItems.Commands.Actions;

internal sealed class StartCommand(IAnsiConsole console, ITextFilterParser filterParser, ITaskItemService service, TaskTitanDbContext dbContext, ILogger<AddCommand> logger) : AsyncCommand<StartCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, StartCommandSettings settings)
    {
        IEnumerable<ITaskQueryFilter> filters = [];
        if (settings.filterText is not null)
        {
            filters = settings.filterText.Select(f => filterParser.Parse(f));
        }
        var tasksToStart = await service.GetTasks(filters);
        var tasktext = "task".ToQuantity(tasksToStart.Count());
        logger.LogInformation("Found {foundTasks} task(s)", tasksToStart.Count());
        foreach (var task in tasksToStart)
        {
            task.Start();
            await service.Update(task);
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Started {foundTasks} task(s)", tasksToStart.Count());

        var text = $"Updated " + tasktext;

        console.WriteLine(text);
        return 0;
    }
}
