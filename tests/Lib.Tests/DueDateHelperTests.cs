using System.Globalization;

namespace TaskTitan.Lib.Tests;

public class DueDateHelperTests
{
    private readonly CultureInfo provider = CultureInfo.InvariantCulture;
    private readonly FakeTimeProvider _timeProvider = new();
    public DueDateHelperTests()
    {

    }
    [Theory]
    [InlineData("2024-01-01")]
    [InlineData("2024-04-16")]
    [InlineData("2024-05-11")]
    [InlineData("2024-08-26")]
    [InlineData("2024-09-22")]
    [InlineData("2024-11-04")]
    public void GivenAStringDateShouldReturnAValidDateOnly(string input)
    {
        FakeTimeProvider fake = new();
        var sut = new DueDateHelper(fake);
        var date = sut.DateStringToDate(input);

        date.Should().NotBeNull("the default format for date strings is 'yyyy-MM-dd'");
    }

    [Theory]
    [InlineData("today", "2024-06-06")]
    [InlineData("yesterday", "2024-06-05")]
    [InlineData("tomorrow", "2024-06-07")]
    [InlineData("monday", "2024-06-10")]
    [InlineData("tuesday", "2024-06-11")]
    [InlineData("wednesday", "2024-06-12")]
    [InlineData("thursday", "2024-06-13")]
    [InlineData("friday", "2024-06-07")]
    [InlineData("saturday", "2024-06-08")]
    [InlineData("sunday", "2024-06-09")]
    [InlineData("eom", "2024-06-30")]
    [InlineData("eoy", "2024-12-31")]
    public void GivenADateSynonymShouldReturnAValidDateOnly(string synonym, string actual)
    {
        // Arrange
        var today = new DateTime(2024, 06, 06);
        _timeProvider.SetUtcNow(new DateTimeOffset(today));
        var sut = new DueDateHelper(_timeProvider);
        var expectedDate = DateTime.ParseExact(actual, "yyyy-MM-dd", provider);

        // Act
        var date = sut.DateSynonymToDate(synonym);

        // Assert
        date.Should().Be(DateOnly.FromDateTime(expectedDate), "a synoymn should correctly convert to a date");
    }
}
