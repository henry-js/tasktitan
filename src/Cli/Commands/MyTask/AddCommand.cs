using Microsoft.Extensions.Logging;

using TaskTitan.Data;
using System.Threading.Tasks;
using TaskTitan.Core;
using TaskTitan.Lib.Services;

namespace TaskTitan.Cli.Commands.TaskCommands;
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
        public override ValidationResult Validate() =>
            string.IsNullOrWhiteSpace(Description)
                ? ValidationResult.Error("Description cannot be empty")
                : base.Validate();
    }
}
