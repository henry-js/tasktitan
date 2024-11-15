using System.CommandLine.Invocation;

using Microsoft.Extensions.Logging;

using TaskTitan.Cli.Display;
using TaskTitan.Core;
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

    new public class Handler(LiteDbContext dBContext, ITaskActionHandler actionHandler, ILogger<NukeCommand> logger) : ICommandHandler
    {
        public string? Filter { get; set; }
        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var query = Filter is not null ? ExpressionParser.ParseFilter(Filter) : null;
            var tasks = dBContext.QueryTasks(query);
            var actionHandlerOptions = new DeleteActionHandlerOptions()
            {
                ActionName = "delete",
                Action = dBContext.DeleteTask
            };
            logger.LogInformation("Deleting {count} tasks", tasks.Count());

            await actionHandler.RunActionWorkflowAsync(tasks, actionHandlerOptions);
            return 0;
        }
    }
}

internal class TaskDeletionOptions
{
    public bool SkipAll { get; set; }
    public bool Interactive => !SkipAll && !ConfirmAll;
    public bool ConfirmAll { get; set; }
}
