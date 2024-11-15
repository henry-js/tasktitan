using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.CommandLine.Invocation;

using TaskTitan.Cli.Display;
using TaskTitan.Core;
using TaskTitan.Core.Configuration;
using TaskTitan.Data;
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Parsers;

namespace TaskTitan.Cli.Commands;

internal sealed class StartCommand : Command
{
    public StartCommand() : base("start", "Begin a task")
    {
        AddOptions(this);
    }

    public static void AddOptions(Command command)
    {
        var filterOption = new Argument<string[]>("Filter", "Filter tasks you want to modify")
        {
            Arity = ArgumentArity.ZeroOrMore
        };
        command.AddArgument(filterOption);

        var modificationOption = new Option<string[]>(["-m", "--mod"], "Changes you want to make to tasks")
        {
            Name = "Modifications",
            Arity = ArgumentArity.ZeroOrMore
        };
        command.AddOption(modificationOption);
    }

    new public class Handler(LiteDbContext dbContext, ITaskActionHandler actionHandler, IOptions<TaskTitanConfig> options, ILogger<StartCommand> logger) : ICommandHandler
    {
        private readonly TaskTitanConfig appConfig = options.Value;

        public string[]? Filter { get; set; }
        public string[]? Modifications { get; set; }
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).Result;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var start = $"start:{DateTime.UtcNow:o}";
            Modifications = Modifications is null ? [start] : [.. Modifications, start];
            var filterExpr = Filter is null ? null : ExpressionParser.ParseFilter(string.Join(' ', Filter));
            var commandExpr = Modifications is null ? new CommandExpression([], "") : ExpressionParser.ParseCommand(string.Join(' ', Modifications));

            if (!commandExpr.Properties.ContainsKey(TaskColumns.Start))
            {
                commandExpr.AddModification("start", DateTime.UtcNow);
                commandExpr.Properties.Add("start", TaskAttributeFactory.CreateBuiltIn(TaskColumns.Start, DateTime.UtcNow));
            }
            var tasks = dbContext.QueryTasks(filterExpr);
            logger.LogInformation("{taskCount} tasks to start.", tasks.Count());

            var actionHandlerOptions = new StartActionHandlerOptions()
            {
                ActionName = "start",
                Action = dbContext.UpdateTask,
                BulkAction = dbContext.BulkUpdateTask,
                Attributes = commandExpr.Properties.Values
            };

            await actionHandler.RunActionWorkflowAsync(tasks, actionHandlerOptions);
            return await Task.FromResult(0);
        }
    }
}
