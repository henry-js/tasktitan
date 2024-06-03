namespace TaskTitan.Cli.AdminCommands;

internal class TestCommandSettings : CommandSettings
{
    [CommandArgument(0, "[name]")]
    public string[] Name { get; set; }
}
