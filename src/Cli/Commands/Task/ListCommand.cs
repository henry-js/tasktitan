using System.Threading.Tasks;

namespace TaskTitan.Cli.TaskCommands;

internal sealed class ListCommand(IAnsiConsole console, ITtaskService service, ILogger<ListCommand> logger) : AsyncCommand<TaskSettings>
{
    private readonly IAnsiConsole console = console;
    private readonly ITtaskService service = service;
    private readonly ILogger<ListCommand> logger = logger;

    public override Task<int> ExecuteAsync(CommandContext context, TaskSettings settings)
    {
        logger.LogDebug("Fetching tasks");
        var pending = service.GetTasks().ToList();
        console.ListPendingTasks(pending);
        return Task.FromResult(0);
    }
}
