using Humanizer;

namespace TaskTitan.Cli.Commands.Actions;

// TODO: Should use a filter to LIST commands first then perform modification
internal sealed class ModifyCommand : Command
{
    public ModifyCommand() : base("modify", "Modify an existing task")
    {
        AddOptions(this);
    }
    public static void AddOptions(Command command)
    {
        var filterOption = new Option<string[]?>(
            aliases: ["-f", "--filter"]
        )
        {
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.ZeroOrMore
        };
        command.AddOption(filterOption);

        var descriptionOption = new Option<string?>(aliases: ["--desc"], ar => string.Join(' ', ar.Tokens))
        {
            Arity = ArgumentArity.OneOrMore
        };
        command.AddOption(descriptionOption);

        var dueOption = new Option<string?>(
            aliases: ["-d", "--due"]
            );
        command.AddOption(dueOption);

        var scheduledOption = new Option<string?>(
            aliases: ["-s", "--sched", "--scheduled"]
            );
        command.AddOption(scheduledOption);

        var waitOption = new Option<string?>(
            aliases: ["-w", "--wait"]
            );
        command.AddOption(waitOption);

        var untilOption = new Option<string?>(
            aliases: ["-u", "--until"]
            );
        command.AddOption(untilOption);
    }

    new public class Handler(IAnsiConsole console, ITaskItemService service, ILogger<ModifyCommand> logger)
    : ICommandHandler
    {
        private readonly ILogger<ModifyCommand> _logger = logger;
        public string[]? Filter { get; set; }
        public string[]? Description { get; set; }
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
            _logger.LogInformation("Handling {Request}", nameof(TaskItemModifyRequest));

            Dictionary<TaskItemAttribute, string> modifiers = [];

            TaskItemModifyRequest request = new()
            {
                Filters = Filter ?? [],
            };
            if (Description is not null)
                request.Attributes.Add(TaskItemAttribute.Description, string.Join(" ", Description));
            if (Due is not null)
                request.Attributes.Add(TaskItemAttribute.Due, Due);
            if (Scheduled is not null)
                request.Attributes.Add(TaskItemAttribute.Scheduled, Scheduled);
            if (Until is not null)
                request.Attributes.Add(TaskItemAttribute.Until, Until);
            if (Wait is not null)
                request.Attributes.Add(TaskItemAttribute.Wait, Wait);

            int rowsUpdated = await service.Update(request);

            console.WriteLine("tasks".ToQuantity(rowsUpdated) + " updated.");
            return 0;
        }
    }
}
