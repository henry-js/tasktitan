using System.CommandLine;
using System.CommandLine.Parsing;
using TaskTitan.Data.Expressions;
using TaskTitan.Cli.Commands;

namespace TaskTitan.Cli.Tests;

public class CommandTests
{
    // [Test]
    // public async Task AddCommandCanParseDescriptionArgument()
    // {
    //     var command = new AddCommand();
    //     string description = "i am a description argument";
    //     var result = command.Parse(description);
    //     var arg = result.CommandResult.Command.Arguments.First() as CliArgument<string>;
    //     var value = result.GetValue<string>(arg);

    //     await Assert.That(value).IsEqualTo(description);
    // }

    // [Test]
    // [Arguments(new string[] { "-m", "due:tomorrow" }, "due:tomorrow")]
    // [Arguments(new string[] { "-m", "due:tomorrow" }, "due:tomorrow")]
    // [Arguments(new string[] { "-m", "due:tomorrow", "-m", "project:work" }, "due:tomorrow project:work")]
    // public async Task AddCommandCanParseModifyOption(string[] modifyText, string expected)
    // {
    //     var command = new AddCommand();
    //     string[] input = ["description placeholder", .. modifyText];
    //     var result = command.Parse(input);

    //     var opt = result.CommandResult.Command.Options.First() as CliOption<CommandExpression>;

    //     var value = result.GetValue(opt!);

    //     await Assert.That(value!.Input).IsEqualTo(expected);
    // }
}
