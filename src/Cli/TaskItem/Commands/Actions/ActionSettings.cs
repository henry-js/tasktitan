namespace TaskTitan.Cli.TaskItem.Commands;

internal class ActionSettings : CommandSettings
{
    [CommandArgument(0, "[id]")]
    public int rowId { get; set; }
}
