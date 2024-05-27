using System.Globalization;

using Humanizer;

using TaskTitan.Cli.Ttask.Models;

namespace TaskTitan.Cli.TaskItem.Commands;

internal static class TTaskConsole
{
    internal static void ListTasks(this IAnsiConsole console, IEnumerable<TtaskDto> tasks)
    {
        var table = new Table()
            .Border(TableBorder.Horizontal)
            .AddColumns("Id", nameof(TTask.Description), nameof(TTask.Due));

        foreach (var task in tasks)
        {
            var humanizedDate = task.Due?.Humanize() ?? "";
            table.AddRow(task.RowId.ToString(CultureInfo.CurrentCulture), task.Description, humanizedDate);
        }

        console.Write(table);
        string taskSummary = "task".ToQuantity(tasks.Count());
        console.Write($"{taskSummary}.");
    }

    internal static void DisplayTask(this IAnsiConsole console, TTask task)
    {

    }
}
