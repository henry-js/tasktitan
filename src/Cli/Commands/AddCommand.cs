using System.CommandLine.Invocation;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TaskTitan.Core;
using TaskTitan.Core.Configuration;
using TaskTitan.Data;
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Parsers;

namespace TaskTitan.Cli.Commands;

public sealed class AddCommand : Command
{
    public AddCommand() : base("add", "Add a task to the list")
    {
        AddOptions(this);
    }

    public static void AddOptions(Command command)
    {
        var descriptionArgument = new Argument<string[]>("Modification")
        {
            Arity = ArgumentArity.OneOrMore,
        };

        command.Add(descriptionArgument);
    }

    new public class Handler(IAnsiConsole console, LiteDbContext dbContext, IOptions<TaskTitanConfig> reportOptions, ILogger<AddCommand> logger) : ICommandHandler
    {
        private readonly TaskTitanConfig reportConfig = reportOptions.Value;
        private readonly IAnsiConsole console = console;
        public CommandExpression Modification { get; set; } = default!;

        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var builder = new StringBuilder().Append("description:'");
            List<string> attributes = [];
            List<string> descValues = [];
            for (int i = 0; i < context.ParseResult.CommandResult.Tokens.Count; i++)
            {
                string current = context.ParseResult.CommandResult.Tokens[i].Value;
                if (current.Contains(':') || current.Contains('+'))
                {
                    attributes.Add(current);
                }
                else
                {
                    descValues.Add(current);
                }
            }
            builder.AppendJoin(' ', descValues).Append('\'').Append(' ').AppendJoin(' ', attributes);
            logger.LogInformation("Parsed raw input");

            ExpressionParser.SetUdas(reportConfig.Uda);

            logger.LogInformation("Parsing command");
            Modification = ExpressionParser.ParseCommand(builder.ToString().Trim());

            logger.LogInformation("Adding task");
            // var taskId = dbContext.AddTask(Modification.Properties);
            var taskId = dbContext.AddTask(Modification.Properties);

            logger.LogInformation("Created task with id {id}", taskId);

            console.MarkupLineInterpolated($"[green]Created task {taskId}[/]");
            return await Task.FromResult(0);
        }
    }
}
