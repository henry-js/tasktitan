using System.Threading.Tasks;

namespace TaskTitan.Cli.TaskItems.Commands.Actions;

internal sealed class StartCommand(IAnsiConsole console, ITaskItemService service, ILogger<AddCommand> logger) : AsyncCommand<ActionSettings>
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

        return System.Threading.Tasks.Task.FromResult(0);
    }
}
