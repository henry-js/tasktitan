using System.Threading.Tasks;

namespace TaskTitan.Cli.AdminCommands;

internal class TestCommand : AsyncCommand<TestCommandSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, TestCommandSettings settings)
    {
        Console.WriteLine(string.Join(' ', settings.Name));
        return Task.FromResult(0);
    }

}
