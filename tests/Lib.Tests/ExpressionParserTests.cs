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
    public void ShouldCorrectlyParseCompoundFilterExpressions(string expression)
    {
        // Arrange
        IExpressionParser sut = new ExpressionParser();

        // Act
        var result = sut.ParseFilter(expression);

        // Assert
        result.Should().BeAssignableTo<GroupedExpression>();
    }
}
