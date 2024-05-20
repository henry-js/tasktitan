using System.Globalization;
using System.Threading.Tasks;

namespace TaskTitan.Cli.TaskCommands;

internal sealed class ModifyCommand(IAnsiConsole console, ITtaskService service, ILogger<ModifyCommand> logger)
: AsyncCommand<ModifyCommand.Settings>
{
    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        TTaskResult? updateResult = default;
        logger.LogInformation("Querying task with rowId: {rowId}", settings.rowId);
        var task = service.Get(settings.rowId);

        if (task == null)
        {
            console.MarkupLineInterpolated(CultureInfo.CurrentCulture, $"Task {settings.rowId} not found");
            return Task.FromResult(-1);
        }
        if (settings.due != null)
        {
            updateResult = service.Update(task);
        }
        console.WriteLine(updateResult?.Success == true ? "Update successful" : "Update failed");
        return Task.FromResult(0);
    }

    internal sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        public int rowId { get; set; }

        [CommandArgument(1, "[due]")]
        public string? due { get; set; }

        public override ValidationResult Validate()
        {
            if (rowId < 1) return ValidationResult.Error("rowId cannot be less than 0");
            return due is not null && !due.StartsWith("due:", StringComparison.OrdinalIgnoreCase)
                ? ValidationResult.Error("due argument correct format: 'due:day'")
                : ValidationResult.Success();
        }
    }
}
