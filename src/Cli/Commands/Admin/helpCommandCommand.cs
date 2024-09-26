using Spectre.Console;
using System.Commandline;
using System.Commandline.Invocation;

namespace MyProjectNamespace.helpCommand;

internal sealed class helpCommandCommand : Command
{
    public AddCommand() : base("add", "Add a task to the list")
    {
        AddOptions(this);
    }

    public static void AddOptions(Command command)
    {
    }

    new public class Handler(IAnsiConsole console, ILogger<AddCommand> logger) : ICommandHandler
    {
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).Result;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            return 0;
        }
    }
}
