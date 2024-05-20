using System.Globalization;

using Humanizer;

namespace TaskTitan.Cli.TaskCommands;

internal static class TTaskConsole
{
    internal static void ListTasks(this IAnsiConsole console, List<TTask> tasks)
    {
        var table = new Table()
            .Border(TableBorder.Horizontal)
            .AddColumns(nameof(TTask.Id), nameof(TTask.Description));

        foreach (var task in tasks)
        {
            table.AddRow(task.Id.Value, task.Description);
        }

        console.Write(table);
    }

    internal static void ListPendingTasks(this IAnsiConsole console, List<TTask> tasks)
    {
        var table = new Table()
            .Border(TableBorder.Horizontal)
            .AddColumns("Id", nameof(TTask.Description));

        foreach (var task in tasks)
        {
            table.AddRow(task.RowId.ToString(CultureInfo.CurrentCulture), task.Description);
        }

        console.Write(table);
        string taskSummary = "task".ToQuantity(tasks.Count);
        console.Write($"{taskSummary}.");
    }
}
