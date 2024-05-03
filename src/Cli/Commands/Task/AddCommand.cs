using Spectre.Console;
using Spectre.Console.Cli;

namespace TaskTitan.Cli.Commands.TaskCommands;
internal class AddCommand(IAnsiConsole console) : AsyncCommand<TaskSettings>
{
    private readonly IAnsiConsole _console = console;

    public override Task<int> ExecuteAsync(CommandContext context, TaskSettings settings)
    {
        _console.WriteLine("Hello from task add");
        return Task.FromResult(0);
    }
}
