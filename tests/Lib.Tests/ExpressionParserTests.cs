using TaskTitan.Lib.Expressions;

using Xunit.Categories;

namespace TaskTitan.Lib.Tests;

[UnitTest]
public class ExpressionParserTests
{
    [Theory]
    [InlineData("status:pending")]
    [InlineData("+work")]
    [InlineData("+home")]
    [InlineData("-test")]
    [InlineData("(due:today or due:tomorrow)")]
    public void ShouldCorrectlyParseAllBuiltInAttributeFilterExpressions(string expression)
    {
        // Arrange
        IExpressionParser sut = new ExpressionParser();
        // Act
        var result = sut.ParseFilter(expression);
        // Assert
    }
}
