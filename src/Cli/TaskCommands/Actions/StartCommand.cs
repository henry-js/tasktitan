using System.CommandLine;
using System.CommandLine.Invocation;

using Humanizer;

using TaskTitan.Core.Enums;

namespace TaskTitan.Cli.TaskCommands.Actions;

internal sealed class StartCommand : Command
{
    public StartCommand() : base("start", "Start an existing task")
    {
        AddOptions(this);
    }

    public static void AddOptions(Command command)
    {
        var filterOption = new Option<string[]>(
            aliases: ["-f", "--filter"]
        )
        {
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.ZeroOrMore
        };
        command.AddOption(filterOption);
    }

    new public class Handler(IAnsiConsole console, ITaskItemService service, ILogger<AddCommand> logger)
    : ICommandHandler
    {
        public string[]? Filter { get; set; }
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).Result;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var request = new TaskItemModifyRequest()
            {
                Filters = Filter ?? []
            };
            request.Attributes.Add(TaskItemAttribute.Start, DateTime.Now);
            logger.LogInformation("Handling {Request}", nameof(TaskItemModifyRequest));

            await service.Update(request);
            var tasksToStart = await service.GetTasks(Filter ?? []);
            logger.LogInformation("Found {foundTasks} task(s)", tasksToStart.Count());
            foreach (var task in tasksToStart)
            {
                task.Begin();
                // await service.Update(task);
            }

            logger.LogInformation("Started {foundTasks} task(s)", tasksToStart.Count());

            var tasktext = "task".ToQuantity(tasksToStart.Count());
            var text = $"Updated " + tasktext;

            console.WriteLine(text);
            return 0;
        }
    }
}
