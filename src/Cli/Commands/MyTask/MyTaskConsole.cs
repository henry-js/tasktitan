namespace TaskTitan.Cli.Commands.TaskCommands;

internal static class MyTaskConsole
{
    internal static void ListTasks(this IAnsiConsole console, List<Core.MyTask> tasks)
    {
        var table = new Table()
        .Border(TableBorder.Horizontal)
        .AddColumns(nameof(Core.MyTask.Id), nameof(Core.MyTask.Description));

        foreach (var task in tasks)
        {
            table.AddRow(task.Id.Value, task.Description);
        }

        console.Write(table);
    }

}
