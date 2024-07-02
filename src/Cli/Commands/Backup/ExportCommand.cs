using System.CommandLine;
using System.CommandLine.Invocation;

using TaskTitan.Infrastructure.ExternalSync;

namespace TaskTitan.Cli.Commands.Backup;

internal class ExportCommand : Command
{
    public ExportCommand(Option<SupportedService> fromOption) : base("export", "Export tasktitan tasks to a supported service")
    {
        AddOption(fromOption);
        AddOptions(this);
    }
    private static void AddOptions(Command command)
    {
    }
    new internal class Handler : ICommandHandler
    {
        public int Invoke(InvocationContext context)
        {
            throw new NotImplementedException();
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
