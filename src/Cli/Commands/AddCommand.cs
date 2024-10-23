using System.CommandLine;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Spectre.Console;
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
        public CommandExpression? Modification { get; set; }

        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var tasks = dbContext.Tasks;
            var task = new TaskItem("Modification.Children");

            tasks.Insert(task);

            var fetchedTask = tasks.FindById(task.Uuid);

            console.WriteLine(JsonSerializer.Serialize(fetchedTask));

            console.WriteLine($"Added task {tasks.Count()}");
            return await Task.FromResult(0);
        }

    }
}
