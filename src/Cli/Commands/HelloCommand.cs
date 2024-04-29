using System.Text.Json;
using Microsoft.Extensions.Options;
using TaskTitan.Lib;
using Spectre.Console;
using Spectre.Console.Cli;

namespace TaskTitan.Cli.Commands;

public class HelloCommand(IAnsiConsole ansiConsole, IOptions<NestedSettings> nestedSetttings) : AsyncCommand<HelloCommand.Settings>
{
    public class Settings : CommandSettings
    {
    }

    private readonly IAnsiConsole _console = ansiConsole;
    private readonly NestedSettings _nestedSettings = nestedSetttings.Value;

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        _console.MarkupLine("[green]Hello[/] [magenta]there![/]");

        _console.MarkupLine($"In appsettings.json value of NestedSettings:");
        _console.WriteLine(JsonSerializer.Serialize(_nestedSettings, new JsonSerializerOptions { WriteIndented = true }));

        return await Task.FromResult(0);
    }
}
