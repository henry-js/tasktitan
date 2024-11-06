using TaskTitan.Core;
using TaskTitan.Data.Parsers;

namespace TaskTitan.Cli.Commands;

public static class CliGlobalOptions
{
    public static readonly Option<CommandExpression?> ModificationOption = new(
        aliases: ["-m", "--modify"],
        description: "Due date etc",
        parseArgument: ar => ExpressionParser.ParseCommand(string.Join(' ', ar.Tokens)))
    {
        AllowMultipleArgumentsPerToken = true,
        Arity = ArgumentArity.OneOrMore
    };

    public static readonly Option<FilterExpression> FilterOption = new(
        aliases: ["-f", "--filter"],
        description: "Filter tasks by",
        parseArgument: ar => ExpressionParser.ParseFilter(string.Join(' ', ar.Tokens)))
    {
        AllowMultipleArgumentsPerToken = true,
        Arity = ArgumentArity.ZeroOrMore,
    };
}
