using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

using TaskTitan.Lib.Dates;

namespace TaskTitan.Cli.TaskItems.Commands;

// TODO: Should use a filter to LIST commands first then perform modification
internal sealed class ModifyCommand(IAnsiConsole console, IStringFilterConverter<DateTime> dateConverter, ITaskItemService service, ILogger<ModifyCommand> logger)
: AsyncCommand<ModifySettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, ModifySettings settings)
    {
        logger.LogInformation("Querying task with rowId: {rowId}", settings.rowId);
        var task = service.Get(settings.rowId);
        Debug.WriteLine(task);
        // ParsedInput input = ParseInput(settings);
        if (task == null)
        {
            console.MarkupLineInterpolated(CultureInfo.CurrentCulture, $"Task {settings.rowId} not found");
            return System.Threading.Tasks.Task.FromResult(-1);
        }
        if (settings.due != null)
        {
            task.Due = dateConverter.ConvertFrom(settings.due);
        }
        // if (settings.wait != null)
        // {
        //     task.Metadata.Wait = dateConverter.ConvertFrom(settings.wait);
        // }
        var updateResult = service.Update(task);
        Debug.WriteLine(task);

        console.WriteLine(updateResult.IsSuccess ? "Update successful" : $"Update failed");
        if (updateResult.ErrorMessages.Length > 0)
            console.WriteLine($@"
Errors:
    {string.Join(Environment.NewLine, updateResult.ErrorMessages)}
");
        return System.Threading.Tasks.Task.FromResult(0);
    }
}
