using System.Globalization;

using TaskTitan.Core;

namespace TaskTitan.Cli.Commands.TaskCommands;

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

    internal static void ListPendingTasks(this IAnsiConsole console, List<PendingTTask> tasks)
    {
        var table = new Table()
            .Border(TableBorder.Horizontal)
            .AddColumns("Id", nameof(PendingTTask.Description));

        foreach (var task in tasks)
        {
            table.AddRow(task.RowId.ToString(CultureInfo.CurrentCulture), task.Description);
        }

        console.Write(table);
    }
}
