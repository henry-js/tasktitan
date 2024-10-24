using System.CommandLine.Invocation;
using System.Text;
using System.Text.Json;

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
        static CommandExpression parse(System.CommandLine.Parsing.ArgumentResult ar)
        {
            var builder = new StringBuilder().Append("desc:'");
            List<string> attributes = [];
            List<string> descValues = [];
            for (int i = 0; i < ar.Tokens.Count; i++)
            {
                string current = ar.Tokens[i].Value;
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
            return ExpressionParser.ParseCommand(builder.ToString());
        }

        var descriptionArgument = new Argument<CommandExpression>("Modification",
        parse: parse)
        {
            Arity = ArgumentArity.OneOrMore,
        };

        command.Add(descriptionArgument);
    }

    new public class Handler(IAnsiConsole console, LiteDbContext dbContext) : ICommandHandler
    {
        private readonly IAnsiConsole console = console;
        public CommandExpression Modification { get; set; } = default!;

        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var taskId = dbContext.AddTask(Modification.Properties);

            return await Task.FromResult(0);
        }
    }
}
