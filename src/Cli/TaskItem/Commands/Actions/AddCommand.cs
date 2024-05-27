using System.Threading.Tasks;

namespace TaskTitan.Cli.TaskItem.Commands.Actions;

internal sealed class AddCommand(IAnsiConsole console, ITtaskService service, ILogger<AddCommand> logger) : AsyncCommand<ModifySettings>
{
    private readonly IAnsiConsole console = console;
    private readonly ITtaskService service = service;
    private readonly ILogger logger = logger;

    public override Task<int> ExecuteAsync(CommandContext context, ModifySettings settings)
    {
        var task = TTask.CreateNew(settings.text);
        var rowid = service.Add(task);

        console.WriteLine($"Created task {rowid}.");
        return Task.FromResult(0);
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

            Scheduled = "Hello i have been edited";
            return base.Validate();
        }
    }
}
