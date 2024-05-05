
using Microsoft.Extensions.Logging;

using Spectre.Console;
using Spectre.Console.Cli;

namespace TaskTitan.Cli.Commands.TaskCommands;
internal class AddCommand(IAnsiConsole console, ILogger<AddCommand> logger) : AsyncCommand<TaskSettings>
{
    private readonly IAnsiConsole _console = console;
    private readonly ILogger _logger = logger;

    public override Task<int> ExecuteAsync(CommandContext context, TaskSettings settings)
    {
        _logger.LogDebug("Add command called");
        _console.WriteLine("Hello from task add");
        return Task.FromResult(0);
    }
}
