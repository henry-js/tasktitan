using System.Threading.Tasks;

namespace TaskTitan.Cli.TaskCommands;

internal sealed class AddCommand(IAnsiConsole console, ITtaskService service, ILogger<AddCommand> logger) : AsyncCommand<AddCommand.Settings>
{
    private readonly IAnsiConsole console = console;
    private readonly ITtaskService service = service;
    private readonly ILogger logger = logger;

    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var task = TTask.CreateNew(settings.Description);
        var rowid = service.Add(task);

        console.WriteLine($"Created task {rowid}.");
        return Task.FromResult(0);
    }

    internal sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<Description>")]
        public string Description { get; set; } = string.Empty;
        [CommandArgument(2, "scheduled")]
        public string Scheduled { get; set; } = string.Empty;
        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Description))
                return ValidationResult.Error("Description cannot be empty.");

            if (!string.IsNullOrWhiteSpace(Scheduled) && Scheduled.StartsWith("scheduled:", StringComparison.Ordinal))
            {
                return ValidationResult.Error("Incorrect syntax for scheduled.");
            }
            else
            {
                Scheduled = "Hello i have been edited";
            }

            return base.Validate();
        }
    }
}
