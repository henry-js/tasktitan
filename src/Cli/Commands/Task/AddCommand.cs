using Serilog;

using Spectre.Console;
using Spectre.Console.Cli;

namespace TaskTitan.Cli.Commands.TaskCommands;
internal class AddCommand(IAnsiConsole console, ILogger logger) : AsyncCommand<TaskSettings>
{
    private readonly IAnsiConsole _console = console;
    private readonly ILogger _logger = logger;

    public override Task<int> ExecuteAsync(CommandContext context, TaskSettings settings)
    {
        _logger.Information("Hello from add command");
        _console.WriteLine("Hello from task add");
        return Task.FromResult(0);
    }
}
