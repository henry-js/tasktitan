using System.CommandLine;
using System.CommandLine.Invocation;

namespace TaskTitan.Cli.Commands.Backup;

internal sealed class ImportCommand : Command
{
    public ImportCommand() : base("import", "Import existing commands from supported service")
    {
        AddOptions(this);
    }
    private static void AddOptions(Command command)
    {
        var fromOption = new Option<ExternalTaskManager>(
            aliases: ["-f", "--from"]
        );
        command.AddOption(fromOption);
    }

    new public class Handler(IAnsiConsole console, ITaskItemService service, ILogger<ImportCommand> logger)
    : ICommandHandler
    {
        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;

        public Task<int> InvokeAsync(InvocationContext context)
        {
            throw new NotImplementedException();
        }
    }
}

internal enum ExternalTaskManager
{
    ToDo
}
