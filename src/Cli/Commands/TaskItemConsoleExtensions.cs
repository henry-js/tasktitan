using System.Diagnostics;

using Humanizer;

using TaskTitan.Cli.Reports;

namespace TaskTitan.Cli.Commands;

internal static class TaskItemConsoleExtensions
{
    internal static void DisplayReport(ReportOptions report, IEnumerable<TaskItem> tasks)
    {
        var grid = new Grid();
        var colHeaders = report.Labels.Split(",");
        grid.AddColumns(colHeaders.Length);

        grid.AddRow(colHeaders);
        foreach (var task in tasks)
        {

        }
    }
    internal static void ListTasks(this IAnsiConsole console, IEnumerable<TaskItem> tasks, IEnumerable<FormattedTaskItemAttribute>? fields = null)
    {
        fields ??= [];
        var grid = new Grid();

        if (!tasks.Any())
        {
            console.MarkupLine("No matches found");
            return;
        }

        grid.AddColumns(fields.Count());
        grid.AddRow(fields.Select(f => (string)f.FieldName).ToArray());
        foreach (var task in tasks)
        {
            AddGridTaskItemRow(grid, task, fields);
            // var humanizedDate = task.Due?.Value.Humanize() ?? "";
            // grid.AddRow([$"{task.RowId}", $"{task.Description}", $"{humanizedDate}"]);
        }

        console.Write(grid);
    }

    private static void AddGridTaskItemRow(Grid grid, TaskItem task, IEnumerable<FormattedTaskItemAttribute> fields)
    {
        var fieldsArr = fields.ToArray();
        string[] row = new string[fieldsArr.Length];
        var props = task.GetType().GetProperties();
        for (int i = 0; i < fieldsArr.Length; i++)
        {
            var prop = props.SingleOrDefault(p => string.Equals(fieldsArr[i].FieldName, p.Name, StringComparison.OrdinalIgnoreCase));
            if (prop is null)
            {
                Console.WriteLine($"Field {fieldsArr[i].FieldName} could not be found on TaskItem object");
                row[i] = "";
                continue;
            }

            var propValue = prop.GetValue(task);
            Debug.WriteLine($"Field: {fieldsArr[i].FieldName}, Name: {prop?.Name}, Value: {prop.GetValue(task)}");

            row[i] = FormatPropValue(propValue, fieldsArr[i]);
        }
        // for (int i = 0; i < props.Length; i++)
        // {
        //     var prop = props[i];
        //     var formatter = fields.SingleOrDefault(f => string.Equals(f.FieldName.Value, prop.Name, StringComparison.OrdinalIgnoreCase));
        //     string propValue = prop.GetValue(task)?.ToString() ?? "";
        //     row[i] = formatter is null ? propValue : FormatPropValue(propValue, formatter);
        //     Debug.WriteLine($"Name: {prop.Name}, Value: {prop.GetValue(task)}");
        // }
        static string FormatPropValue(object? value, FormattedTaskItemAttribute formatter)
        {
            if (value is null) return "";

            object notNullVal = value!;
            return formatter.Format switch
            {
                FieldFormat.None => value.ToString() ?? "",
                FieldFormat.Date => ((TaskDate)value).DateOnly.ToString(),
                FieldFormat.Indicator => value.ToString()![0].ToString(),
                FieldFormat.Age => (((TaskDate)value) - DateTime.UtcNow).Days.ToString(),
                _ => value.ToString() ?? "",
            };
        }

        grid.AddRow(row);
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

        return choice == yes || choice == all;
    }
}
