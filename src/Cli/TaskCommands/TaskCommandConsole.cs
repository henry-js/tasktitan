using Humanizer;

using TaskTitan.Lib.Dtos;

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

    internal static void DisplayTask(this IAnsiConsole console, Core.TaskItem task)
    {

    }
}
