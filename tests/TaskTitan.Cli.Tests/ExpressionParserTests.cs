using TUnit.Assertions.Extensions.Generic;
using TaskTitan.Data.Parsers;
using TaskTitan.Data.Expressions;
using System.Text.Json;
using static TaskTitan.Data.Enums;

namespace TaskTitan.Cli.Tests;

public class PidginParserTests
{
    [Test]
    [Arguments("due:tomorrow", "due", "tomorrow")]
    [Arguments("until:2w", "until", "2w")]
    [Arguments("project:home", "project", "home")]
    [Arguments("project:WORK", "project", "WORK")]
    [Arguments("due:2024-01-02T00:00:00", "due", "2024-01-02T00:00:00")]

    public async Task AnAttributePairCanBeParsedFromText(string text, string keyText, string value)
    {
        var result = ExpressionParser.ParseFilter(text);


        TaskProperty attribute = (result.Expr as TaskProperty)!;

        await Assert.That(attribute.Name).IsEqualTo(keyText);
        await Assert.That(attribute.Modifier).IsNull();
    }

    [Test]
    [Arguments("due:8w and until:7w", BinaryOperator.And)]
    [Arguments("due:9w until:8w", BinaryOperator.And)]
    [Arguments("due:10w or until:9w", BinaryOperator.Or)]
    [Arguments("project:work or project:notWork", BinaryOperator.Or)]
    public async Task ABinaryExpressionCanBeParsedFromText(string text, BinaryOperator @operator)
    {
        var result = ExpressionParser.ParseFilter(text);

        // await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Expr).IsAssignableTo(typeof(BinaryFilter));

        var resultVal = result.Expr as BinaryFilter;
        await Assert.That(resultVal?.Operator).IsEquivalentTo(@operator);
    }

    [Test]
    [Arguments("+test", ColModifier.Include)]
    [Arguments("-test", ColModifier.Exclude)]
    public async Task ATagExpressionCanBeParsedFromText(string tagText, ColModifier modifier)
    {
        var result = ExpressionParser.ParseFilter(tagText);

        await Assert.That(result.Expr).IsAssignableTo(typeof(TaskTag));
        var tag = result.Expr as TaskTag;
        await Assert.That(tag?.Modifier).IsEqualTo(modifier);
    }

    [Test]
    [Arguments("due:tomorrow", typeof(TaskProperty))]
    [Arguments("+test or due:tomorrow", typeof(BinaryFilter))]
    [Arguments("due:tomorrow or project:home", typeof(BinaryFilter))]
    [Arguments("project:work and until:1w or due:monday", typeof(BinaryFilter))]
    public async Task DifferentExpressionsCanBeParsedFromText(string text, Type t)
    {
        var result = ExpressionParser.ParseFilter(text);
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true });
        await Assert.That(result.Expr).IsAssignableTo(t);
    }

    [Test]
    [Arguments("due:tomorrow", 1)]
    [Arguments("due:tomorrow until:tuesday", 2)]
    [Arguments("due:tomorrow until:tuesday project:work", 3)]
    [Arguments("due:tomorrow until:tuesday project:work +fun", 4)]
    public async Task CommandExpressionCanBeParsedFromText(string text, int quantity)
    {
        var result = ExpressionParser.ParseCommand(text);

        await Assert.That(result).IsNotNull();

        await Assert.That(result.Children).HasCount().EqualTo(quantity);
    }
}
