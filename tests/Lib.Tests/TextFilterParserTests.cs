using BenchmarkDotNet.Attributes;

using TaskTitan.Lib.Text;

using Xunit.Categories;

namespace TaskTitan.Lib.Tests;

public class TextFilterParserTests
{
    private readonly CultureInfo provider = CultureInfo.InvariantCulture;
    public TextFilterParserTests() { }

    [Theory]
    [InlineData("123")]
    [InlineData("123,456")]
    [InlineData("123-456")]
    [InlineData("123,456-789")]
    [InlineData("123-456,789")]
    [InlineData("123,456,789")]
    [InlineData("123-456,789-1011")]
    [InlineData("123,456-789,101")]
    [InlineData("1,2,3")]
    [InlineData("10,20-30,40,50-60")]
    [InlineData("100-200,300,400-500")]
    [InlineData("5,10-20,25-30,35")]
    [InlineData("999")]
    [InlineData("1000-2000")]
    [InlineData("1000,2000-3000")]
    [InlineData("1,2-3,4,5-6")]
    [InlineData("1234-5678")]
    [InlineData("1,2-3,4-5,6")]
    [InlineData("12,34-56,78,90-100")]
    [InlineData("10-20,30,40-50,60,70-80")]
    public void GivenAStringIdQueryFilterShouldReturnAnIdRangeFilter(string input)
    {
        // Arrange
        ITextFilterParser sut = new TextFilterParser();

        // Act
        ITaskQueryFilter filter = sut.Parse(input);
        IdQueryFilter rangeFilter = (IdQueryFilter)filter;
        var filterParamCount = rangeFilter.SoleIds.Count + rangeFilter.IdRange.Count;

        // Assert
        filter.Should().BeAssignableTo<IdQueryFilter>();
        filterParamCount.Should().Be(input.Split(',').Length);
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
