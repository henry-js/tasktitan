using System.Threading.Tasks;

namespace TaskTitan.Cli.TaskCommands;

internal sealed class ModifyCommand(IAnsiConsole console, ITtaskService service, ILogger<AddCommand> logger) : AsyncCommand<AddCommand.Settings>
{
    public override Task<int> ExecuteAsync(CommandContext context, AddCommand.Settings settings)
    {
        throw new NotImplementedException();
    }
}
