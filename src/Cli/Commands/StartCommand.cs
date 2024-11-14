using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Spectre.Console;

using System.CommandLine;
using System.CommandLine.Invocation;

using TaskTitan.Core;
using TaskTitan.Core.Configuration;
using TaskTitan.Data;
using TaskTitan.Data.Parsers;

namespace TaskTitan.Cli.Commands;

internal sealed class StartCommand : Command
{
    public StartCommand() : base("start", "Begin a task")
    {
        AddOptions(this);
    }

    public static void AddOptions(Command command)
    {
        var filterOption = new Argument<string[]>("Filter", "Filter tasks you want to modify")
        {
            Arity = ArgumentArity.ZeroOrMore
        };
        command.AddArgument(filterOption);

        var modificationOption = new Option<string[]>(["-m", "--mod"], "Changes you want to make to tasks")
        {
            Name = "Modifications",
            Arity = ArgumentArity.ZeroOrMore
        };
        command.AddOption(modificationOption);
    }

    new public class Handler(LiteDbContext dbContext, IAnsiConsole console, IOptions<TaskTitanConfig> options, ILogger<StartCommand> logger) : ICommandHandler
    {
        private readonly TaskTitanConfig appConfig = options.Value;

        public string[]? Filter { get; set; }
        public string[]? Modifications { get; set; }
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).Result;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var start = DateTime.UtcNow;
            var filterExpr = ExpressionParser.ParseFilter(string.Join(' ', Filter));
            var commandExpr = ExpressionParser.ParseFilter(string.Join(' ', Modifications));

            var tasks = dbContext.QueryTasks(filterExpr);
            logger.LogInformation("{taskCount} tasks to start.", tasks.Count());

            await TaskActionHandler.RunActionWorkflowAsync(tasks, new ActionHandlerOptions(), dbContext.DeleteTask);
            console.WriteLine("In start command");
            return await Task.FromResult(0);
        }
    }
}

public static class TaskActionHandler
{
    public static async Task RunActionWorkflowAsync(IEnumerable<TaskItem> tasks, ActionHandlerOptions options, Func<TaskItem, Task<bool>> deleteTask)
    {
        AnsiConsole.WriteLine($"This command will delete {tasks.Count()} tasks.");
        foreach ((int index, var task) in tasks.Index())
        {
            if (await ShouldDeleteTaskAsync(task, options))
            {
                try
                {
                    AnsiConsole.WriteLine($"Deleting task {task.Id}");
                    await deleteTask(task);
                    // logger.LogInformation("Deleted task {taskId}: {taskDescription}", task.Id, task.Description);
                }
                catch (Exception ex)
                {
                    // logger.LogError(ex, "Failed to delete task {taskId}", task.Id);
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


    private static async Task<bool> ShouldContinueAfterErrorAsync()
    {
        return await Task.FromResult(true);
    }

    private static async Task<bool> ShouldDeleteTaskAsync(TaskItem task, ActionHandlerOptions options)
    {
        if (options.ConfirmAll) return true;
        if (options.SkipAll) return false;
        if (options.Interactive)
        {
            string[] choices = ["yes", "no", "all", "quit"];
            var answer = await new TextPrompt<string>($"Delete task {task.Id} '{task.Description}'? ({string.Join('/', choices)})", StringComparer.OrdinalIgnoreCase)
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

public class ActionHandlerOptions
{
    public bool SkipAll { get; set; }
    public bool Interactive => !SkipAll && !ConfirmAll;
    public bool ConfirmAll { get; set; }
}
