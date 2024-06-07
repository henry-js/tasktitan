using TaskTitan.Lib.Expressions;

using Xunit.Categories;

using static TaskTitan.Data.DbConstants.KeyWords;
using static TaskTitan.Data.DbConstants.TasksTable;
namespace TaskTitan.Lib.Tests;

[UnitTest]
public class ExpressionParserTests
{
    private readonly DateTime _today;
    private readonly TimeProvider _timeProvider;

    public ExpressionParserTests()
    {
        _today = new DateTime(2024, 06, 14);
        _timeProvider = new FakeTimeProvider(_today);
    }
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
        result.Should().BeAssignableTo<TagFilterExpression>();
    }

    [Theory]
    [InlineData("status:pending", "status")]
    [InlineData("due:tomorrow", "due")]
    [InlineData("recur:daily", "recur")]
    [InlineData("until:1w", "until")]
    [InlineData("limit:1000", "limit")]
    [InlineData("wait:5d", "wait")]
    [InlineData("end:eom", "end")]
    [InlineData("start:yesterday", "start")]
    [InlineData("scheduled:eoy", "scheduled")]
    public void ShouldCorrectlyParseAttributeFilterExpressions(string expression, string expectedKey)
    {
        IExpressionParser sut = new ExpressionParser();

        // Act
        var result = sut.ParseFilter(expression);

        // Assert
        result.Should().BeAssignableTo<AttributeFilterExpression>();
        var af = result as AttributeFilterExpression;
        af!.attribute.Should().BeEquivalentTo(expectedKey);
        af.Value.Should().NotBeNull();

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
        result.Should().BeAssignableTo<GroupedFilterExpression>();
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
    [InlineData("1", $"({RowId} IN (1))")]
    [InlineData("5-9", $"({RowId} BETWEEN 5 AND 9)")]
    [InlineData("5-20,1,3,5", $"({RowId} IN (1,3,5)) OR ({RowId} BETWEEN 5 AND 20)")]
    [InlineData("7,5,1-4,2-7,1,3,5", $"({RowId} IN (1,3,5,7)) OR ({RowId} BETWEEN 1 AND 4) OR ({RowId} BETWEEN 2 AND 7)")]
    public void GivenAStringIdQueryFilterShouldConvertToValidSql(string input, string expected)
    {
        // Arrange
        IExpressionParser sut = new ExpressionParser();

        // Act
        var result = sut.ParseFilter(input) as IdFilterExpression;

        // Assert
        result!.ToQueryString().Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("due:eom", $"date(due) = date('2024-06-30')")]
    [InlineData("due:tomorrow", $"date(due) = date('2024-06-15')")]
    [InlineData("due:today", $"date(due) = date('2024-06-14')")]
    [InlineData("due:yesterday", $"date(due) = date('2024-06-13')")]
    public void GivenAStringAttributeFilterShouldConvertToValidSql(string input, string expected)
    {
        // Arrange
        IExpressionParser sut = new ExpressionParser();
        var dtConverter = new DateTimeConverter(_timeProvider);
        var options = new AttributeFilterConversionOptions() { StandardDateConverter = dtConverter };
        // Act
        var result = sut.ParseFilter(input) as AttributeFilterExpression;

        // Assert
        result!.ToQueryString(options).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("(due:eom and status:pending)", $"(date(due) = date('2024-06-30')) AND (status = 'Pending')")]
    [InlineData("(project:home or due:tomorrow)", $"(project = 'home') OR (date(due) = date('2024-06-15'))")]
    [InlineData("(modified:yesterday and due:today)", $"(date(modified) = date('2024-06-13')) AND (date(due) = date('2024-06-14'))")]
    public void GivenAGroupAttributeFilterShouldConvertToValidSql(string input, string expected)
    {
        // Arrange
        IExpressionParser sut = new ExpressionParser();
        var dtConverter = new DateTimeConverter(_timeProvider);
        var options = new AttributeFilterConversionOptions() { StandardDateConverter = dtConverter };
        // Act
        var result = sut.ParseFilter(input) as GroupedFilterExpression;

        // Assert
        result!.ToQueryString(options).Should().BeEquivalentTo(expected);
    }
}
