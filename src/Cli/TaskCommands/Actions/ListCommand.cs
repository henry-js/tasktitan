using System.Threading.Tasks;

using TaskTitan.Lib.Dtos;

// using static TaskTitan.Cli.TaskCommands.TaskItemConsole;

namespace TaskTitan.Cli.TaskCommands;

internal sealed class ListCommand(IAnsiConsole console, ITaskItemService service, ILogger<ListCommand> logger) : AsyncCommand<ListSettings>
{
    private readonly IAnsiConsole console = console;
    private readonly ITaskItemService service = service;
    private readonly ILogger<ListCommand> logger = logger;

    public override async Task<int> ExecuteAsync(CommandContext context, ListSettings settings)
    {
        logger.LogInformation("Received filter: {filters}", string.Join(", ", settings.filterText ?? []));
        var tasks = settings.filterText is null ? await service.GetTasks([]) : await service.GetTasks(settings.filterText);

        if (tasks.Count() == 1)
        {
            console.DisplayTaskDetails(tasks.First());
        }
        else
        {
            console.ListTasks(tasks.Select(TaskItemDto.FromTaskItem));
        }

        return 0;
    }
}

internal class ListSettings : CommandSettings
{
    [CommandOption("-f|--filter")]
    public string[]? filterText { get; set; }
}
