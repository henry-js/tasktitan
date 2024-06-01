namespace TaskTitan.Cli.TaskItems.Commands;

internal class StartCommandSettings : CommandSettings
{
    [CommandArgument(1, "[filter]")]
    public string[]? filterText { get; set; }
}
