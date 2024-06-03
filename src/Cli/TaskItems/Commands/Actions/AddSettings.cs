
namespace TaskTitan.Cli.TaskItems.Commands;

internal class AddSettings : CommandSettings
{
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

    public override ValidationResult Validate()
    {
        if (Description.Length == 0) return ValidationResult.Error("Task description cannot be empty");
        return ValidationResult.Success();
    }
}
