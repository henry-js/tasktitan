using System.Globalization;

using TaskTitan.Lib.Dates;

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
        var sut = new DateParser(fake);
        var success = sut.IsExactDate(input, out DateOnly? date);

        success.Should().BeTrue();
        date.Should().NotBeNull("the default format for date strings is 'yyyy-MM-dd'");
        var dateParts = input.Split("-").Select(s => Convert.ToInt32(s)).ToList();
        dateParts[0].Should().Be(date?.Year);
        dateParts[1].Should().Be(date?.Month);
        dateParts[2].Should().Be(date?.Day);
    }

    [Theory]
    [InlineData("today", "2024-06-06")]
    [InlineData("yesterday", "2024-06-05")]
    [InlineData("tomorrow", "2024-06-07")]

    [InlineData("eom", "2024-06-30")]
    [InlineData("eoy", "2024-12-31")]
    public void GivenARelativeSynonymShouldReturnAValidDateOnly(string synonym, string expected)
    {
        // Arrange
        var today = new DateTime(2024, 06, 06);
        _timeProvider.SetUtcNow(new DateTimeOffset(today));
        var sut = new DateParser(_timeProvider);
        var exact = DateTime.ParseExact(expected, "yyyy-MM-dd", provider);

        // Act
        var success = sut.IsRelative(synonym, out DateOnly? date);

        // Assert
        success.Should().BeTrue();
        date.Should().Be(DateOnly.FromDateTime(exact), "a synoymn should correctly convert to a date");
    }

    [Theory]
    [InlineData("monday", "2024-06-10")]
    [InlineData("tuesday", "2024-06-11")]
    [InlineData("wednesday", "2024-06-12")]
    [InlineData("thursday", "2024-06-13")]
    [InlineData("friday", "2024-06-07")]
    [InlineData("saturday", "2024-06-08")]
    [InlineData("sunday", "2024-06-09")]
    public void GivenADayoOfWeekShouldReturnDateFromToday(string dayOfWeek, string expected)
    {
        // Arrange
        var today = new DateTime(2024, 06, 06);
        _timeProvider.SetUtcNow(new DateTimeOffset(today));
        var sut = new DateParser(_timeProvider);
        var exact = DateTime.ParseExact(expected, "yyyy-MM-dd", provider);

        // Act
        var success = sut.IsNextDate(dayOfWeek, out DateOnly? date);

        // Assert
        success.Should().BeTrue();
        date.Should().Be(DateOnly.FromDateTime(exact), "a synoymn should correctly convert to a date");

    }
}
