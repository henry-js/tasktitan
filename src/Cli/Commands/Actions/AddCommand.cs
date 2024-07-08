using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace TaskTitan.Cli.Commands.Actions;

internal sealed class AddCommand : Command
{
    public AddCommand() : base("add", "Add a task to the list")
    {
        AddOptions(this);
    }

    public static void AddOptions(Command command)
    {
        var descriptionArgument = new Argument<string>("description"
        , parse: ar => string.Join(' ', ar.Tokens)
        )
        {
            Arity = ArgumentArity.OneOrMore,
        };
        command.AddArgument(descriptionArgument);
        var dueOption = new Option<string?>(
            aliases: ["-d", "--due"],
            description: "When task is due"
        );
        command.AddOption(dueOption);

        var scheduledOption = new Option<string?>(
            aliases: ["-s", "--scheduled"],
            description: "When to schedule"

        );
        command.AddOption(scheduledOption);

        var waitOption = new Option<string?>(
            aliases: ["-w", "--wait"],
            description: "how long until task is shown"
        );
        command.AddOption(waitOption);

        var untilOption = new Option<string?>(
            aliases: ["-u", "--until"],
            description: "When to hide task"
        );
        command.AddOption(untilOption);
    }

    new public class Handler(IAnsiConsole console, ITaskItemService service, IStringFilterConverter<TaskDate> stringConverter, ILogger<AddCommand> logger) : ICommandHandler
    {
        public required string Description { get; set; }
        public string? Due { get; set; }
        public string? Scheduled { get; set; }
        public string? Wait { get; set; }
        public string? Until { get; set; }

        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).Result;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            logger.LogInformation("Handling {Request}", nameof(TaskItemCreateRequest));
            var task = TaskItem.CreateNew(Description);
            task.Due = stringConverter.ConvertFrom(Due);
            task.Scheduled = stringConverter.ConvertFrom(Scheduled);
            task.Wait = stringConverter.ConvertFrom(Wait);
            task.Until = stringConverter.ConvertFrom(Until);
            TaskItemCreateRequest request = new()
            {
                Task = task
            };
            var rowid = await service.Add(request);

            console.WriteLine($"Created task {rowid}.");
            return 0;
        }
    }
}
