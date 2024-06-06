using TaskTitan.Lib.Expressions;

using Xunit.Categories;

using static TaskTitan.Data.DbConstants.KeyWords;
using static TaskTitan.Data.DbConstants.TasksTable;
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
    [InlineData("(-home or 1,2,3)")]
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
    [InlineData("5", 1, 0)]
    [InlineData("5,6", 2, 0)]
    [InlineData("2-6", 0, 1)]
    [InlineData("2,6-8", 1, 1)]
    [InlineData("2-6,8", 1, 1)]
    [InlineData("2,6,8", 3, 0)]
    [InlineData("2-6,8-10", 0, 2)]
    [InlineData("2,6-8,10", 2, 1)]
    [InlineData("1,2,3", 3, 0)]
    [InlineData("10,20-30,40,44-50", 2, 2)]
    [InlineData("5,10-20,25-30,35", 2, 2)]
    [InlineData("11-22", 0, 1)]
    [InlineData("11,22-40", 1, 1)]
    [InlineData("1,2-3,4,5-6", 2, 2)]
    [InlineData("1,2-3,4-5,6", 2, 2)]
    [InlineData("10-20,30,40-50,60,70-80", 2, 3)]
    public void ShouldCorrectlyParseIdFilterExpression(string expression, int expectedSoleIdCount, int expectedRangeCount)
    {
        // Arrange
        IExpressionParser sut = new ExpressionParser();

        // Act
        var result = sut.ParseFilter(expression);

        // Assert
        result.Should().BeAssignableTo<IdFilterExpression>();
        var exp = result as IdFilterExpression;
        exp!.Ids.Should().HaveCount(expectedSoleIdCount);
        exp!.Ranges.Should().HaveCount(expectedRangeCount);

    }

    [Theory]
    [InlineData("1", $"{RowId} IN (1)")]
    [InlineData("5-9", $"{RowId} BETWEEN 5 AND 9")]
    [InlineData("5-20,1,3,5", $"{RowId} IN (1,3,5) OR {RowId} BETWEEN 5 AND 20")]
    [InlineData("7,5,1-4,2-7,1,3,5", $"{RowId} IN (1,3,5,7) OR {RowId} BETWEEN 1 AND 4 OR {RowId} BETWEEN 2 AND 7")]
    public void GivenAStringIdQueryFilterShouldConvertToValidSql(string input, string expected)
    {
        // Arrange
        IExpressionParser sut = new ExpressionParser();

        // Act
        var result = sut.ParseFilter(input) as IdFilterExpression;

        // Assert
        result!.ToQueryString().Should().BeEquivalentTo(expected);
    }
}
