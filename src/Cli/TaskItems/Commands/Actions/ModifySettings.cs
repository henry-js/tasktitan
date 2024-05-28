namespace TaskTitan.Cli.TaskItems.Commands;

internal sealed class ModifySettings : ActionSettings
{
    [CommandOption("-t|--text")]
    public string text { get; set; } = string.Empty;
    [CommandOption("-d|--due")]
    public string? due { get; set; }
    [CommandOption("-s|--sched|--scheduled")]
    public string? scheduled { get; set; }
    [CommandOption("-w|--wait")]
    public string? wait { get; internal set; }
    [CommandOption("-u|--until")]
    public string? until { get; internal set; }

    public override ValidationResult Validate()
    {
        if (rowId < 1) return ValidationResult.Error("rowId cannot be less than 0");
        return ValidationResult.Success();
    }
}
