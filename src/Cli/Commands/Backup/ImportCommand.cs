using System.CommandLine;
using System.CommandLine.Invocation;

namespace TaskTitan.Cli.Commands.Backup;

// TODO: Should use a filter to LIST commands first then perform modification
internal sealed class ImportCommand : Command
{
    public ImportCommand() : base("import", "Import existing commands from supported service")
    {
        AddOptions(this);
    }
    private static void AddOptions(Command command)
    {
        var fromOption = new Option<TaskManagementService>(
            aliases: ["-f", "--from"]
        );
        command.AddOption(fromOption);
    }

    new public class Handler(IAnsiConsole console, ITaskItemService service, ILogger<ImportCommand> logger)
    : ICommandHandler
    {
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).Result;
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            throw new NotImplementedException();
        }
    }
}

internal enum TaskManagementService
{
    ToDo
}
