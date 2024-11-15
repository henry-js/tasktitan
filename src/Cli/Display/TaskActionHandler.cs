using System.Runtime.CompilerServices;

using Microsoft.Extensions.Logging;

using TaskTitan.Core;

namespace TaskTitan.Cli.Display;

public interface ITaskActionHandler
{
    Task RunActionWorkflowAsync(IEnumerable<TaskItem> tasks, ActionHandlerOptions options);
}

public class TaskActionHandler(ILogger<TaskActionHandler> logger) : ITaskActionHandler
{
    public async Task RunActionWorkflowAsync(IEnumerable<TaskItem> tasks, ActionHandlerOptions options)
    {
        string action = options.ActionName;

        AnsiConsole.WriteLine($"This command will {action} {tasks.Count()} tasks.");
        foreach ((int index, var task) in tasks.Select((task, index) => (index, task)))
        {
            if (await ShouldExecuteActionAsync(task, options))
            {
                try
                {
                    AnsiConsole.WriteLine($"{action}ing task {task.Id}");
                    bool success = options switch
                    {
                        DeleteActionHandlerOptions deleteOptions => await deleteOptions.Action.Invoke(task),
                        StartActionHandlerOptions startOptions => await startOptions.Action.Invoke(task.ApplyBuiltIn(startOptions.Attributes)),
                        _ => throw new SwitchExpressionException(),
                    };

                    logger.LogInformation("{action}d task {taskId}: {taskDescription}", action, task.Id, task.Description);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to {action} task {taskId}", action, task.Id);
                    if (!await ShouldContinueAfterErrorAsync())
                    {
                        break;
                    }
                }
            }
            if (options.SkipAll)
            {
                AnsiConsole.WriteLine($"Skipped {tasks.Count() - index} tasks");
                break;
            }
        }
    }

    private async Task<bool> ShouldContinueAfterErrorAsync()
    {
        return await Task.FromResult(true);
    }

    private async Task<bool> ShouldExecuteActionAsync(TaskItem task, ActionHandlerOptions options)
    {
        if (options.ConfirmAll) return true;
        if (options.SkipAll) return false;
        if (options.Interactive)
        {
            string[] choices = ["yes", "no", "all", "quit"];
            var answer = await new TextPrompt<string>($"{options.ActionName} task {task.Id} '{task.Description}'? ({string.Join('/', choices)})", StringComparer.OrdinalIgnoreCase)
                .Validate((userInput) => choices.Any(choice => choice.StartsWith(userInput)))
                .ShowAsync(AnsiConsole.Console, CancellationToken.None);

            switch (answer[0])
            {
                case 'q':
                    options.SkipAll = true;
                    return false;
                case 'a':
                    options.ConfirmAll = true;
                    return true;
                case 'n':
                    return false;
                case 'y':
                    return true;
            }
        }

        return false;
    }
}
