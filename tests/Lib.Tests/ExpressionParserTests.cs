using TaskTitan.Lib.Expressions;

using Xunit.Categories;

namespace TaskTitan.Lib.Tests;

[UnitTest]
public class ExpressionParserTests
{
    [Theory]
    [InlineData("+work")]
    [InlineData("+home")]
    [InlineData("-test")]
    public void ShouldCorrectlyParseTagFilterExpressions(string expression)
    {
        // Arrange
        IExpressionParser sut = new ExpressionParser();

        // Act
        var result = sut.ParseFilter(expression);

        // Assert
        result.Should().BeAssignableTo<TagFilter>();
    }

    [Theory]
    [InlineData("status:pending")]
    [InlineData("due:tomorrow")]
    [InlineData("recur:daily")]
    [InlineData("until:1w")]
    [InlineData("limit:1000")]
    [InlineData("wait:5d")]
    [InlineData("end:eom")]
    [InlineData("start:yesterday")]
    [InlineData("scheduled:eoy")]
    public void ShouldCorrectlyParseAttributeFilterExpressions(string expression)
    {
        // Arrange
        IExpressionParser sut = new ExpressionParser();

        // Act
        var result = sut.ParseFilter(expression);

        // Assert
        result.Should().BeAssignableTo<AttributeFilter>();

    }

    [Theory]
    [InlineData("(due:today or due:tomorrow)")]
    [InlineData("(scheduled:wednesday and due:thursday)")]
    [InlineData("(scheduled:wednesday and +home)")]
    [InlineData("(-home or end:eom)")]
    public void ShouldCorrectlyParseCompoundFilterExpressions(string expression)
    {
        // Arrange
        IExpressionParser sut = new ExpressionParser();

        // Act
        var result = sut.ParseFilter(expression);

        // Assert
        result.Should().BeAssignableTo<GroupedExpression>();
    }

    [Theory]
    [InlineData("5")]
    [InlineData("5,6")]
    [InlineData("2-6")]
    [InlineData("2,6-8")]
    [InlineData("2-6,8")]
    [InlineData("2,6,8")]
    [InlineData("2-6,8-10")]
    [InlineData("2,6-8,10")]
    [InlineData("1,2,3")]
    [InlineData("10,20-30,40,44-50")]
    [InlineData("5,10-20,25-30,35")]
    [InlineData("11-22")]
    [InlineData("11,22-40")]
    [InlineData("1,2-3,4,5-6")]
    [InlineData("1,2-3,4-5,6")]
    [InlineData("10-20,30,40-50,60,70-80")]
    public void ShouldCorrectlyParseIdFilterExpression(string expression)
    {
        // Arrange
        IExpressionParser sut = new ExpressionParser();

        // Act
        var result = sut.ParseFilter(expression);

        // Assert
        result.Should().BeAssignableTo<IdFilterExpression>();

    }
}
