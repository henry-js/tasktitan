namespace TaskTitan.Cli.TaskCommands;

internal class TaskSettings : CommandSettings
{
    [CommandArgument(0, "[taskNum]")]
    public int? taskNum { get; set; }

    public override ValidationResult Validate()
    {
        if (taskNum is not null && taskNum < 1) return ValidationResult.Error("taskNum cannot be less than 1");
        return ValidationResult.Success();
    }
}
