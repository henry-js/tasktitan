using System.CommandLine.Invocation;

using Microsoft.Extensions.Logging;

using TaskTitan.Core;
using TaskTitan.Data;
using TaskTitan.Data.Parsers;


namespace TaskTitan.Cli.Commands;

internal sealed class NukeCommand : Command
{
    public NukeCommand() : base("nuke", "Delete all tasks in database")
    {
        AddOptions(this);
        IsHidden = true;
    }

    public static void AddOptions(Command command)
    {
    }

    new public class Handler(LiteDbContext dBContext, IAnsiConsole console, ILogger<NukeCommand> logger) : ICommandHandler
    {
        public string? Filter { get; set; }
        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var query = Filter is not null ? ExpressionParser.ParseFilter(Filter) : null;
            var tasks = dBContext.QueryTasks(query);
            var options = new TaskDeletionOptions();
            await RunDeletionWorkflowAsync(tasks, options);
            return 0;
        }

        public async Task RunDeletionWorkflowAsync(IEnumerable<TaskItem> tasks, TaskDeletionOptions options)
        {
            console.WriteLine($"This command will delete {tasks.Count()} tasks.");
            foreach (var task in tasks)
            {
                if (await ShouldDeleteTaskAsync(task, options))
                {
                    try
                    {
                        console.WriteLine($"Deleting task {task.Id}");
                        await dBContext.NukeTaskAsync(task);
                        logger.LogInformation("Deleted task {taskId}: {taskDescription}", task.Id, task.Description);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to delete task {taskId}", task.Id);
                        if (!await ShouldContinueAfterErrorAsync())
                        {
                            break;
                        }
                    }
                }
            }
        }

        private async Task<bool> ShouldContinueAfterErrorAsync()
        {
            return true;
        }

        private async Task<bool> ShouldDeleteTaskAsync(TaskItem task, TaskDeletionOptions options)
        {
            if (options.ConfirmAll) return true;

            if (options.Interactive)
            {
                var prompt = new TextPrompt<string>($"Delete task {task.Id} '{task.Description}'?", StringComparer.OrdinalIgnoreCase)
                .AddChoices(["yes", "no", "all", "quit"])
                .WithConverter(input => input);

                var answer = console.Prompt(prompt);
                switch (answer)
                {
                    case "quit":
                        options.SkipAll = true;
                        return false;
                    case "all":
                        options.ConfirmAll = true;
                        return true;
                    case "no":
                        return false;
                    case "yes":
                        return true;
                }
            }

            return false;
        }
    }
}

internal struct TaskDeletionOptions
{
    public bool SkipAll { get; set; }
    public readonly bool Interactive => !SkipAll && !ConfirmAll;
    public bool ConfirmAll { get; set; }
}
