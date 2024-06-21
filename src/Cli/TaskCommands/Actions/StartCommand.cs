using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

using Humanizer;

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
            IEnumerable<Expression> filters = [];
            var tasksToStart = await service.GetTasks(Filter ?? []);
            var tasktext = "task".ToQuantity(tasksToStart.Count());
            logger.LogInformation("Found {foundTasks} task(s)", tasksToStart.Count());
            foreach (var task in tasksToStart)
            {
                task.Begin();
                // await service.Update(task);
            }

            logger.LogInformation("Started {foundTasks} task(s)", tasksToStart.Count());

            var text = $"Updated " + tasktext;

            console.WriteLine(text);
            return 0;
        }
    }
}
