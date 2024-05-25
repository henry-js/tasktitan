using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

using TaskTitan.Lib.Dates;

namespace TaskTitan.Cli.TaskCommands;

internal sealed class ModifyCommand(IAnsiConsole console, IDateTimeConverter dateConverter, ITtaskService service, ILogger<ModifyCommand> logger)
: AsyncCommand<ModifyCommand.Settings>
{
    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        logger.LogInformation("Querying task with rowId: {rowId}", settings.rowId);
        var task = service.Get(settings.rowId, false);
        Debug.WriteLine(task);
        // ParsedInput input = ParseInput(settings);
        if (task == null)
        {
            console.MarkupLineInterpolated(CultureInfo.CurrentCulture, $"Task {settings.rowId} not found");
            return Task.FromResult(-1);
        }
        if (settings.due != null)
        {
            task.DueDate = dateConverter.ConvertFrom(settings.due);
        }
        if (settings.wait != null)
        {
            task.Metadata.Wait = dateConverter.ConvertFrom(settings.wait);
        }
        var updateResult = service.Update(task);
        Debug.WriteLine(task);

        console.WriteLine(updateResult.IsSuccess ? "Update successful" : $"Update failed");
        if (updateResult.ErrorMessages.Length > 0)
            console.WriteLine($@"
Errors:
    {string.Join(Environment.NewLine, updateResult.ErrorMessages)}
");
        return Task.FromResult(0);
    }

    internal sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "[id]")]
        public int rowId { get; set; }

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
            if (rowId < 1) return ValidationResult.Error("rowId cannot be less than 0");
            return ValidationResult.Success();
        }
    }
}
