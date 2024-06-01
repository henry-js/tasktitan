using System.Threading.Tasks;

using TaskTitan.Cli.TaskItems.Models;
using TaskTitan.Lib.Text;

namespace TaskTitan.Cli.TaskItems.Commands;

internal sealed class ListCommand(IAnsiConsole console, ITextFilterParser filterParser, ITaskItemService service, ILogger<ListCommand> logger) : AsyncCommand<ListSettings>
{
    private readonly IAnsiConsole console = console;
    private readonly ITaskItemService service = service;
    private readonly ILogger<ListCommand> logger = logger;

    public override async Task<int> ExecuteAsync(CommandContext context, ListSettings settings)
    {
        if (settings.filterText is null)
        {
            logger.LogDebug("Fetching all pending tasks");
            var pending = (await service.GetTasks()).Select(t => TaskItemDto.FromTaskItem(t));

            console.ListTasks(pending);
            return 0;
        }
        var filters = settings.filterText.Select(f => filterParser.Parse(f));

        var tasks = await service.GetTasks(filters);
        console.ListTasks(tasks.Select(t => TaskItemDto.FromTaskItem(t)));

        return 0;
    }
}

internal sealed class ListSettings : CommandSettings
{
    [CommandArgument(1, "[filter]")]
    public string[]? filterText { get; set; }
}
