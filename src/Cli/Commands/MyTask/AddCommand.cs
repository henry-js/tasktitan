using Microsoft.Extensions.Logging;

using TaskTitan.Data;
using System.Threading.Tasks;
using TaskTitan.Core;

namespace TaskTitan.Cli.Commands.TaskCommands;
internal sealed class AddCommand(IAnsiConsole console, TaskTitanDbContext dbContext, ILogger<AddCommand> logger) : AsyncCommand<AddCommand.Settings>
{
    private readonly IAnsiConsole console = console;
    private readonly TaskTitanDbContext dbContext = dbContext;
    private readonly ILogger logger = logger;

    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var task = TTask.CreateNew(settings.Description);
        dbContext.Add(task);

        console.WriteLine("Hello from task add");
        return Task.FromResult(0);
    }
    internal class Settings : CommandSettings
    {
        [CommandArgument(0, "<Description>")]
        public string Description { get; set; } = string.Empty;
        public override ValidationResult Validate() =>
            string.IsNullOrWhiteSpace(Description)
                ? ValidationResult.Error("Description cannot be empty")
                : base.Validate();
    }
}
