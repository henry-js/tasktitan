namespace TaskTitan.Cli.TaskCommands;

internal sealed class ModifySettings : CommandSettings
{
    [CommandOption("-f|--filter")]
    public string[]? filterText { get; set; }
    [CommandArgument(0, "[description]")]
    public string[] Description { get; set; } = [];
    [CommandOption("-d|--due")]
    public string? due { get; set; }
    [CommandOption("-s|--sched|--scheduled")]
    public string? scheduled { get; set; }
    [CommandOption("-w|--wait")]
    public string? wait { get; internal set; }
    [CommandOption("-u|--until")]
    public string? until { get; internal set; }
}
