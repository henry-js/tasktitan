namespace TaskTitan.Cli.TaskItems.Commands;

internal class ActionSettings : CommandSettings
{
    [CommandArgument(0, "[id]")]
    public int rowId { get; set; }

}

internal class StartCommandSettings : CommandSettings
{
    [CommandArgument(1, "[filter]")]
    public string[]? filterText { get; set; }
}
