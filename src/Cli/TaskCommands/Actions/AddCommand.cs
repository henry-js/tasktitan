using System.Threading.Tasks;

using TaskTitan.Core.Enums;

namespace TaskTitan.Cli.TaskCommands.Actions;

internal sealed class AddCommand(IAnsiConsole console, ITaskItemService service, ILogger<AddCommand> logger) : AsyncCommand<AddSettings>
{
    private readonly IAnsiConsole console = console;
    private readonly ITaskItemService service = service;
    private readonly ILogger logger = logger;

    public override async Task<int> ExecuteAsync(CommandContext context, AddSettings settings)
    {
        TaskItemCreateRequest request = new()
        {
            NewTask = new()
            {
                Description = string.Join(' ', settings.Description),
                Due = settings.due,
                Scheduled = settings.scheduled,
                Wait = settings.wait,
                Until = settings.until,
            }
        };
        var rowid = await service.Add(request);

        console.WriteLine($"Created task {rowid}.");
        return 0;
    }

    internal sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<description>")]
        public string Description { get; set; } = string.Empty;

        [CommandArgument(1, "[due]")]
        public string? Due { get; set; }


        [CommandArgument(2, "[scheduled]")]
        public string Scheduled { get; set; } = string.Empty;

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Description))
                return ValidationResult.Error("Description cannot be empty.");

            if (!string.IsNullOrWhiteSpace(Scheduled) && Scheduled.StartsWith("scheduled:", StringComparison.Ordinal))
            {
                return ValidationResult.Error("Incorrect syntax for scheduled.");
            }

            return base.Validate();
        }
    }
}
