using Microsoft.Extensions.Logging;

using Spectre.Console;

using System.CommandLine;
using System.CommandLine.Invocation;

using TaskTitan.Core;

namespace TaskTitan.Cli.Commands;

internal sealed class StartCommand : Command
{
    public StartCommand() : base("add", "Add a task to the list")
    {
        AddOptions(this);
    }

    public static void AddOptions(Command command)
    {
        var filterOption = new Option<string[]>("Filter", "Filter tasks you want to modify")
        {
            Arity = ArgumentArity.ZeroOrMore
        };
        command.AddOption(filterOption);

        var modificationOption = new Option<string[]>("Modifications", "Changes you want to make to tasks")
        {
            Arity = ArgumentArity.ZeroOrMore
        };
        command.AddOption(modificationOption);
    }

    new public class Handler(IAnsiConsole console, ILogger<StartCommand> logger) : ICommandHandler
    {
        public string[]? Filter { get; set; }
        public string[]? Modifications { get; set; }
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).Result;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            console.WriteLine("In start command");
            logger.LogInformation("In start command");
            return await Task.FromResult(0);
        }
    }
}
