using Microsoft.Extensions.Logging;
using TaskTitan.Data;
using System.Threading.Tasks;
using TaskTitan.Core;

namespace TaskTitan.Cli.Commands.TaskCommands;

internal sealed class ListCommand(IAnsiConsole console, TaskTitanDbContext dbContext, ILogger<ListCommand> logger) : AsyncCommand<TaskSettings>
{
    private readonly IAnsiConsole console = console;
    private readonly TaskTitanDbContext dbContext = dbContext;
    private readonly ILogger<ListCommand> logger = logger;

    public override Task<int> ExecuteAsync(CommandContext context, TaskSettings settings)
    {
        logger.LogDebug("Fetching tasks");
        var pending = dbContext.PendingTasks.ToList();
        console.ListPendingTasks(pending);
        return Task.FromResult(0);
    }
}
