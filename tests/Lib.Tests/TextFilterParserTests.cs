using BenchmarkDotNet.Attributes;
using static TaskTitan.Data.DbConstants.TasksTable;
using TaskTitan.Core.Queries;
using TaskTitan.Lib.Queries;
using TaskTitan.Lib.Text;

using Xunit.Categories;

namespace TaskTitan.Lib.Tests;

public class TextFilterParserTests
{
    private readonly CultureInfo provider = CultureInfo.InvariantCulture;
    public TextFilterParserTests() { }

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
    public void GivenAStringIdQueryFilterShouldReturnAnIdRangeFilter(string input)
    {
        // Arrange
        ITextFilterParser sut = new TextFilterParser();

        // Act
        ITaskQueryFilter filter = sut.Parse(input);
        IdQueryFilter rangeFilter = (IdQueryFilter)filter;
        var filterParamCount = rangeFilter.SoleIds.Count + rangeFilter.IdRange.Count();

        // Assert
        filter.Should().BeAssignableTo<IdQueryFilter>();
        filterParamCount.Should().Be(input.Split(',').Length);
    }

    [Theory]
    [InlineData("1", $"{RowId} IN (1)")]
    [InlineData("5-9", $"{RowId} BETWEEN 5 AND 9")]
    [InlineData("5-20,1,3,5", $"{RowId} IN (1,3,5) OR {RowId} BETWEEN 5 AND 20")]
    [InlineData("7,5,1-4,2-7,1,3,5", $"{RowId} IN (1,3,5,7) OR {RowId} BETWEEN 1 AND 4 OR {RowId} BETWEEN 2 AND 7")]
    public void GivenAStringIdQueryFilterShouldConvertToValidSql(string input, string expected)
    {
        // Arrange
        ITextFilterParser sut = new TextFilterParser();

        // Act
        ITaskQueryFilter filter = sut.Parse(input);

        // Assert
        filter.ToQueryString().Should().BeEquivalentTo(expected);
    }
    // [Theory]
    // [InlineData("4,3f-s,11", 2)]
    // [InlineData("23,", 1)]
    // [InlineData("-456", 1)]
    // [InlineData("123,4a6-574", 1)]
    // public void GivenAnIncorrectStringIdFilterShouldReturnNull(string input, int soleIdCount)
    // {
    //     // Arrange
    //     ITextFilterParser sut = new TextFilterParser();

    //     // Act
    //     ITaskQueryFilter filter = sut.Parse(input);
    //     IdQueryFilter rangeFilter = (IdQueryFilter)filter;

    //     // Assert
    //     rangeFilter.SoleIds.Count.Should().Be(soleIdCount);
    // }
}
