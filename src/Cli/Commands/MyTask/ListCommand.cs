using Microsoft.Extensions.Logging;

using TaskTitan.Data;

using static TaskTitan.Core.MyTask;

namespace TaskTitan.Cli.Commands.TaskCommands;

internal sealed class ListCommand(IAnsiConsole console, TaskTitanDbContext dbContext, ILogger<ListCommand> logger) : AsyncCommand<TaskSettings>
{
    private readonly IAnsiConsole console = console;
    private readonly TaskTitanDbContext dbContext = dbContext;

    public override Task<int> ExecuteAsync(CommandContext context, TaskSettings settings)
    {
        var tasks = dbContext.Tasks.Where(t => t.State != TaskState.Done);
        console.ListTasks(dbContext.Tasks.ToList());
        return Task.FromResult(0);
    }
}
