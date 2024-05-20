using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;

using TaskTitan.Lib.Dates;

namespace TaskTitan.Cli.TaskCommands;

internal sealed class ModifyCommand(IAnsiConsole console, ITtaskService service, ILogger<ModifyCommand> logger)
: AsyncCommand<ModifyCommand.Settings>
{
    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        logger.LogInformation("Querying task with rowId: {rowId}", settings.rowId);
        var task = service.Get(settings.rowId, false);

        if (task == null)
        {
            console.MarkupLineInterpolated(CultureInfo.CurrentCulture, $"Task {settings.rowId} not found");
            return Task.FromResult(-1);
        }
        if (settings.Due != null)
        {
            task.DueDate = settings.Due;
            var updateResult = service.Update(task);
            console.WriteLine(updateResult.IsSuccess ? "Update successful" : $"Update failed: {updateResult.Messaqge}");
        }
        return Task.FromResult(0);
    }

    internal sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        public int rowId { get; set; }

        [TypeConverter(typeof(DueDateConverter))]
        [CommandArgument(1, "[due]")]
        public DateOnly? Due { get; set; } = DateOnly.MinValue;

        public override ValidationResult Validate()
        {
            if (rowId < 1) return ValidationResult.Error("rowId cannot be less than 0");
            return ValidationResult.Success();
            // return due is not null && !due.StartsWith("due:", StringComparison.OrdinalIgnoreCase)
            //     ? ValidationResult.Error("due argument correct format: 'due:day'")
            //     : ValidationResult.Success();
        }
    }
}
