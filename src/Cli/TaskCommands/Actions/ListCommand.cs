using System.Threading.Tasks;

using TaskTitan.Cli.TaskCommands.Models;
using TaskTitan.Lib.Expressions;
using TaskTitan.Lib.Text;

namespace TaskTitan.Cli.TaskCommands;

internal sealed class ListCommand(IAnsiConsole console, IExpressionParser expressionParser, ITextFilterParser filterParser, ITaskItemService service, ILogger<ListCommand> logger) : AsyncCommand<ListSettings>
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
        var expressions = settings.filterText.Select(expressionParser.ParseFilter);
        foreach (var exp in expressions)
        {
            console.WriteLine(exp.ToString());
        }
        return 0;
        var filters = settings.filterText.Select(f => filterParser.Parse(f));

        var tasks = await service.GetTasks(filters);
        console.ListTasks(tasks.Select(t => TaskItemDto.FromTaskItem(t)));

        return 0;
    }
}

internal class ListSettings : CommandSettings
{
    [CommandOption("-f|--filter")]
    public string[]? filterText { get; set; }
}
