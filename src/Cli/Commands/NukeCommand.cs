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
            foreach ((int index, var task) in tasks.Index())
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
                if (options.SkipAll)
                {
                    console.WriteLine($"Skipped {tasks.Count() - index} tasks");
                    break;
                }
            }
        }

        private async Task<bool> ShouldContinueAfterErrorAsync()
        {
            return await Task.FromResult(true);
        }

        private async Task<bool> ShouldDeleteTaskAsync(TaskItem task, TaskDeletionOptions options)
        {
            if (options.ConfirmAll) return true;
            if (options.SkipAll) return false;
            if (options.Interactive)
            {
                string[] choices = ["yes", "no", "all", "quit"];
                var answer = await new TextPrompt<string>($"Delete task {task.Id} '{task.Description}'? ({string.Join('/', choices)})", StringComparer.OrdinalIgnoreCase)
                    .Validate((userInput) => choices.Any(choice => choice.StartsWith(userInput)))
                    .ShowAsync(console, CancellationToken.None);

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
}

internal class TaskDeletionOptions
{
    public bool SkipAll { get; set; }
    public bool Interactive => !SkipAll && !ConfirmAll;
    public bool ConfirmAll { get; set; }
}
