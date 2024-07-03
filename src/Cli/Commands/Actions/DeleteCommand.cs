using System.CommandLine;
using System.CommandLine.Invocation;

using Humanizer;

using TaskTitan.Core.OperationResults;

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

            var result = await service.Delete(request);

            if (result.IsFailure)
            {
                console.WriteLine("Delete tasks failed");
                return result.Error.Code;
            }

            var quantity = "task".ToQuantity(result.Value);
            console.MarkupLineInterpolated($"[green]Deleted {quantity}[/]");

            return 0;
        }
    }
}
