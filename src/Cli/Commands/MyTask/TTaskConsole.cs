namespace TaskTitan.Cli.Commands.TaskCommands;

internal static class TTaskConsole
{
    internal static void ListTasks(this IAnsiConsole console, List<Core.TTask> tasks)
    {
        var table = new Table()
        .Border(TableBorder.Horizontal)
        .AddColumns(nameof(Core.TTask.Id), nameof(Core.TTask.Description));

        foreach (var task in tasks)
        {
            table.AddRow(task.Id.Value, task.Description);
        }

        console.Write(table);
    }

}
