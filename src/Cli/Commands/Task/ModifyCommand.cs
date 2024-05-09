using System.Threading.Tasks;

namespace TaskTitan.Cli.TaskCommands;

internal sealed class ModifyCommand(IAnsiConsole console, ITtaskService service, ILogger<ModifyCommand> logger)
: AsyncCommand<ModifyCommand.Settings>
{
    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        logger.LogInformation("Querying task with rowId: {rowId}", settings.rowId);
        var task = service.Get(settings.rowId);

        if (task == null)
        {
            logger.LogInformation("Not found");
            console.MarkupLine("[red]Not found.[/]");
            return Task.FromResult(-1);
        }

        TTaskResult updateTaskResult = service.Update(settings.rowId, settings.dueDate);

        return Task.FromResult(0);
    }

    internal sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        public int rowId { get; set; }

        public string? dueDate { get; set; }
    }
}
