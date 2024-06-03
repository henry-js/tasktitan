using System.Globalization;

using Humanizer;

using TaskTitan.Cli.TaskCommands.Models;

namespace TaskTitan.Cli.TaskCommands;

internal static class TaskItemConsole
{
    internal static void ListTasks(this IAnsiConsole console, IEnumerable<TaskItemDto> tasks)
    {
        if (!tasks.Any())
        {
            console.MarkupLine("No matches found");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Horizontal)
            .AddColumns("Id", nameof(Core.TaskItem.Description), nameof(Core.TaskItem.Due));

        foreach (var task in tasks)
        {
            var humanizedDate = task.Due?.Humanize() ?? "";
            table.AddRow(task.RowId.ToString(CultureInfo.CurrentCulture), task.Description, humanizedDate);
        }

        console.Write(table);
        string taskSummary = "task".ToQuantity(tasks.Count());
        console.Write($"{taskSummary}.");
    }

    internal static void DisplayTask(this IAnsiConsole console, Core.TaskItem task)
    {

    }
}
