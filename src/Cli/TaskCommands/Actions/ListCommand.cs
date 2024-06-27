using System.CommandLine;
using System.CommandLine.Invocation;

using TaskTitan.Lib.Dtos;

namespace TaskTitan.Cli.TaskCommands;

internal sealed class ListCommand : Command
{
    public ListCommand() : base("list", "List tasks in default collection")
    {
        AddOptions(this);
    }

    public static void AddOptions(Command command)
    {
        var filterOption = new Option<string[]?>(
            aliases: ["-f", "--filter"],
            description: "Filter the query"
        );

        command.AddOption(filterOption);
    }

    new public class Handler(IAnsiConsole console, ITaskItemService service, ILogger<ListCommand> logger) : ICommandHandler
    {
        public string[]? Filter { get; set; }

        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).Result;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            logger.LogInformation("Handling {Request}", nameof(TaskItemCreateRequest));
            logger.LogInformation("Received filter: {filters}", string.Join(", ", Filter ?? []));
            var tasks = Filter is null ? await service.GetTasks([]) : await service.GetTasks(Filter);

            if (tasks.Count() == 1)
            {
                console.DisplayTaskDetails(tasks.First());
            }
            else
            {
                console.ListTasks(tasks.Select(TaskItemDto.FromTaskItem));
            }

            return 0;
        }
    }
}
