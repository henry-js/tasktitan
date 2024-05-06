using Microsoft.Extensions.Logging;

using TaskTitan.Core;
using TaskTitan.Data;

namespace TaskTitan.Cli.Commands.TaskCommands;
internal sealed class AddCommand(IAnsiConsole console, TaskTitanDbContext dbContext, ILogger<AddCommand> logger) : AsyncCommand<TaskAddSettings>
{
    private readonly IAnsiConsole console = console;
    private readonly TaskTitanDbContext dbContext = dbContext;
    private readonly ILogger logger = logger;

    public override Task<int> ExecuteAsync(CommandContext context, TaskAddSettings settings)
    {
        var task = MyTask.CreateNew(settings.Description);
        dbContext.Add(task);

        console.WriteLine("Hello from task add");
        return Task.FromResult(0);
    }
}

internal class TaskAddSettings : CommandSettings
{
    [CommandArgument(0, "<Description>")]
    public string Description { get; set; } = string.Empty;
    public override ValidationResult Validate() =>
        string.IsNullOrWhiteSpace(Description)
            ? ValidationResult.Error("Description cannot be empty")
            : base.Validate();
}
