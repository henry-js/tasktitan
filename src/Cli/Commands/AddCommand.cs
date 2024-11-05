using System.CommandLine.Invocation;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Options;

using TaskTitan.Configuration;
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
        // static CommandExpression parse(System.CommandLine.Parsing.ArgumentResult ar)
        // {
        //     var builder = new StringBuilder().Append("description:'");
        //     List<string> attributes = [];
        //     List<string> descValues = [];
        //     for (int i = 0; i < ar.Tokens.Count; i++)
        //     {
        //         string current = ar.Tokens[i].Value;
        //         if (current.Contains(':') || current.Contains('+'))
        //         {
        //             attributes.Add(current);
        //         }
        //         else
        //         {
        //             descValues.Add(current);
        //         }
        //     }
        //     builder.AppendJoin(' ', descValues).Append('\'').Append(' ').AppendJoin(' ', attributes);
        //     return ExpressionParser.ParseCommand(builder.ToString());
        // }

        var descriptionArgument = new Argument<string[]>("Modification")
        {
            Arity = ArgumentArity.OneOrMore,
        };

        command.Add(descriptionArgument);
    }

    new public class Handler(IAnsiConsole console, LiteDbContext dbContext, IOptions<ReportConfiguration> reportOptions) : ICommandHandler
    {
        private readonly ReportConfiguration reportConfig = reportOptions.Value;
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
            ExpressionParser.SetUdas(reportConfig.UDAs);
            Modification = ExpressionParser.ParseCommand(builder.ToString());

            var taskId = dbContext.AddTask(Modification.Properties);

            console.MarkupLineInterpolated($"[green]Created task {taskId}[/]");
            return await Task.FromResult(0);
        }
    }
}
