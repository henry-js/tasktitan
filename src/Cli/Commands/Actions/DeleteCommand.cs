using System.CommandLine;
using System.CommandLine.Invocation;

using Humanizer;

namespace TaskTitan.Cli.Commands.Actions;

internal sealed class DeleteCommand : Command
{
    public DeleteCommand() : base("delete", "delete a task to the list")
    {
        AddOptions(this);
    }

    public static void AddOptions(Command command)
    {
        var filterOption = new Option<string[]>(
            aliases: ["-f", "--filter"],
            description: "Filter tasks to delete")
        {
            Arity = ArgumentArity.ZeroOrMore,
        };
        command.AddOption(filterOption);
    }

    new public class Handler(IAnsiConsole console, ITaskItemService service, ILogger<DeleteCommand> logger) : ICommandHandler
    {
        public required string[] Filter { get; set; }

        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).Result;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            logger.LogInformation("Handling {Request}", nameof(TaskItemCreateRequest));

            TaskItemDeleteRequest request = new()
            {
                Filters = Filter ?? [],
            };

            var fetchedTasks = await service.GetTasks(Filter ?? []);
            int deletedCount = 0;
            int failedCount = 0;
            int skippedCount = 0;
            bool all = false;
            foreach (var task in fetchedTasks)
            {
                if (!all)
                {
                    var delete = console.ConfirmDelete(task, out all);
                    if (!delete)
                    {
                        skippedCount++;
                        continue;
                    }
                }

                var result = await service.Delete(task);

                if (result.IsFailure)
                {
                    logger.LogError("Failed to delete task {TaskId}", task.Id);
                    failedCount++;
                    continue;
                }
                deletedCount++;
                logger.LogInformation("Deleted task {TaskId}", task.Id);
            }

            var deletedQuantity = "task".ToQuantity(deletedCount);
            console.MarkupLineInterpolated($"[green]deleted {deletedQuantity}[/]");
            var skippedQuantity = "task".ToQuantity(skippedCount);
            console.MarkupLineInterpolated($"[yellow]skipped {skippedQuantity}[/]");
            var failedQuantity = "task".ToQuantity(failedCount);
            console.MarkupLineInterpolated($"[red]failed to delete {failedQuantity}[/]");
            return 0;
        }
    }
}
