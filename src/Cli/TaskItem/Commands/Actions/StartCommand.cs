using System.Threading.Tasks;

namespace TaskTitan.Cli.TaskItem.Commands.Actions;

internal sealed class StartCommand(IAnsiConsole console, ITtaskService service, ILogger<AddCommand> logger) : AsyncCommand<ActionSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, ActionSettings settings)
    {
        var task = service.Get(settings.rowId, false);

        if (task is not null)
        {
            task.Start();
            var result = service.Update(task);
            string resultText = result.IsSuccess ? "success" : "failure";
            console.WriteLine($"Update {resultText}");
        }

        return Task.FromResult(0);
    }
}
