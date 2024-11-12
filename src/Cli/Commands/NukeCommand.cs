using System.CommandLine.Invocation;

using Microsoft.Extensions.Logging;

using TaskTitan.Data;
using TaskTitan.Data.Parsers;


namespace TaskTitan.Cli.Commands;

internal sealed class NukeCommand : Command
{
    public NukeCommand() : base("nuke", "Delete all tasks in database")
    {
        AddOptions(this);
        IsHidden = true;
    }

    public static void AddOptions(Command command)
    {
    }

    new public class Handler(LiteDbContext dBContext, IAnsiConsole console, ILogger<NukeCommand> logger) : ICommandHandler
    {
        public string? Filter { get; set; }
        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var query = ExpressionParser.ParseFilter(Filter);
            var tasks = dBContext.QueryTasks(query);


            return 0;
        }
    }
}
