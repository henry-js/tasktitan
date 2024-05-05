using TaskTitan.Data;

namespace TaskTitan.Cli.Commands.TaskCommands;

internal sealed class ListCommand(IAnsiConsole console, TaskTitanDbContext dbContext) : AsyncCommand<TaskSettings>
{
    private readonly IAnsiConsole console = console;

    private readonly TaskTitanDbContext dbContext = dbContext;


    public override Task<int> ExecuteAsync(CommandContext context, TaskSettings settings)
    {
        dbContext.Tasks.ToList();

        console.ListTasks(dbContext.Tasks.ToList());
        return Task.FromResult(0);
    }
}
