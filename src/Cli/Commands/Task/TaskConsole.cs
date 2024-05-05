namespace TaskTitan.Cli.Commands.TaskCommands;

internal static class TaskConsole
{
    internal static void ListTasks(this IAnsiConsole console, List<Core.Task> tasks)
    {
        var table = new Table()
        .Border(TableBorder.Horizontal)
        .AddColumns(nameof(Core.Task.Id), nameof(Core.Task.Description));

        foreach (var task in tasks)
        {
            table.AddRow(task.Id.Value, task.Description);
        }

        console.Write(table);
    }

}
