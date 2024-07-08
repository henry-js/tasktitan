using Humanizer;

using TaskTitan.Infrastructure.Dtos;

namespace TaskTitan.Cli.Commands;

internal static class TaskItemConsoleExtensions
{
    internal static void ListTasks(this IAnsiConsole console, IEnumerable<TaskItem> tasks)
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
            var humanizedDate = task.Due?.Value.Humanize() ?? "";
            grid.AddRow([$"{task.RowId}", $"{task.Description}", $"{humanizedDate}"]);
        }

        console.Write(grid);
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

    internal static bool ConfirmDelete(this IAnsiConsole console, TaskItem task, out bool deleteAll)
    {
        const string yes = "yes";
        const string no = "no";
        const string all = "all";
        var choice = console.Prompt(
            new TextPrompt<string>($"Delete task {task.RowId}, '{task.Description}'?")
                .InvalidChoiceMessage("[red]Invalid option[/]")
                .DefaultValue(yes)
                .AddChoices([yes, no, all])
        );
        deleteAll = choice == all;

        return choice == yes;
    }
}
