using Humanizer;

using TaskTitan.Infrastructure.Dtos;

namespace TaskTitan.Cli.Commands;

internal static class TaskItemConsoleExtensions
{
    internal static void ListTasks(this IAnsiConsole console, IEnumerable<TaskItemDto> tasks)
    {
        if (!tasks.Any())
        {
            console.MarkupLine("No matches found");
            return;
        }

        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();
        grid.AddColumn();

        grid.AddRow(["Id", nameof(TaskItem.Description), nameof(TaskItem.Due)]);
        foreach (var task in tasks)
        {
            var humanizedDate = task.Due?.Humanize() ?? "";
            grid.AddRow([$"{task.RowId}", $"{task.Description}", $"{humanizedDate}"]);
        }

        console.Write(grid);
        // string taskSummary = "task".ToQuantity(tasks.Count());
        // console.Write($"{taskSummary}.");
    }

    internal static void DisplayTaskDetails(this IAnsiConsole console, TaskItem task)
    {
        var grid = new Grid();
        grid.AddColumns(2);
        grid.AddRow([new Markup("Name", new Style(decoration: Decoration.Underline | Decoration.Bold)), new Markup("Value", new Style(decoration: Decoration.Underline | Decoration.Bold))]);
        var props = task.GetType().GetProperties();
        var blue = new Style(background: Color.Blue);
        var red = new Style(background: Color.Red);
        var alternator = true;

        var table = new Table();
        table.AddColumns([new TableColumn("Name"), new TableColumn("Value")]);

        foreach (var prop in props)
        {
            string val = "";
            object? value = prop.GetValue(task);
            val = value is DateTime date ? $"{date} ({date.Humanize()})" : value?.ToString() ?? "";
            var style = alternator ? blue : red;
            grid.AddRow(new Text(prop.Name, style), new Text(val, style));
            var row = new TableRow([new Text(prop.Name, style), new Text(val, style)]);
            table.AddRow(row);
            alternator = !alternator;
        }

        console.Write(grid);
        console.Write(table);
    }
}
