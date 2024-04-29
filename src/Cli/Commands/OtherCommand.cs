using TaskTitan.Lib.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace TaskTitan.Cli.Commands;

public class OtherCommand(IAnsiConsole ansiConsole, ISampleService service) : AsyncCommand
{
    private readonly IAnsiConsole _console = ansiConsole;
    private readonly ISampleService _service = service;

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        _console.MarkupLine("[red]I am the other command[/] [blue]that has no options or flags.[/]");
        _console.MarkupLine("[yellow]But I do use the SampleService[/]");

        _service.DoWork();
        return await Task.FromResult(0);
    }
}
