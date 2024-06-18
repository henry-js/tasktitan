namespace TaskTitan.Cli.TaskCommands;

internal class StartCommandSettings : CommandSettings
{
    [CommandArgument(1, "[filter]")]
    public string[]? filterText { get; set; }
}
