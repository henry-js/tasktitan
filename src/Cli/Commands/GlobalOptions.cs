using System.CommandLine;
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Parsers;

namespace TaskTitan.Cli.Commands;

public static class CliGlobalOptions
{
    // public static readonly CliOption<CommandExpression?> ModificationOption = new("--modify", ["-m"])
    // {
    //     Description = "Due date etc",
    //     CustomParser = ar => ExpressionParser.ParseCommand(string.Join(' ', ar.Tokens)),
    //     AllowMultipleArgumentsPerToken = true,
    //     Arity = ArgumentArity.OneOrMore
    // };

    // public static readonly CliOption<FilterExpression> FilterOption = new("--filter", ["-f"])
    // {
    //     Description = "Filter tasks by",
    //     CustomParser = ar => ExpressionParser.ParseFilter(string.Join(' ', ar.Tokens)),
    //     AllowMultipleArgumentsPerToken = true,
    //     Arity = ArgumentArity.ZeroOrMore,
    //     Recursive = true,
    // };

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
