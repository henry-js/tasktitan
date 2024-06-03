using Xunit.Categories;

namespace TaskTitan.Lib.Tests;

[UnitTest]
public class ExpressionParserTests
{
    [Theory]
    [InlineData("status:pending")]
    [InlineData("+work")]
    [InlineData("+home")]
    [InlineData("(due:today or due:tomorrow)")]
    public void ShouldCorrectlyParseAllBuiltInAttributeFilterExpressions(string expression)
    {
        // Arrange
        var sut = new ExpressionParser();
        // Act
        var result = sut.ParseFilter(expression);
        // Assert
        result.ExpresssionType.Should().Be(ExpressionType.Filter);
    }
}
