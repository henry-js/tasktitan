namespace TaskTitan.Infrastructure.Tests;

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
        var sut = new DateTimeConverter(fake);
        var date = sut.ConvertFrom(input);

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
    [InlineData("eod", "2024-06-06 23:59:59")]
    public void GivenARelativeSynonymShouldReturnAValidDateOnly(string synonym, string expected)
    {
        // Arrange
        var today = new DateTime(2024, 06, 06);
        var gmt = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        _timeProvider.SetLocalTimeZone(gmt);
        _timeProvider.SetUtcNow(today);
        _timeProvider.GetLocalNow();
        var sut = new DateTimeConverter(_timeProvider);
        DateTime.TryParse(expected, provider, DateTimeStyles.AssumeLocal, out var exact);

        // Act
        var date = sut.ConvertFrom(synonym);

        // Assert
        date.Should().Be(exact, "a synoymn should correctly convert to a date");
        date!.Value.Kind.Should().Be(DateTimeKind.Local);
    }

    [Theory]
    [InlineData("monday", "2024-06-10")]
    [InlineData("tuesday", "2024-06-11")]
    [InlineData("wednesday", "2024-06-12")]
    [InlineData("thursday", "2024-06-13")]
    [InlineData("friday", "2024-06-07")]
    [InlineData("saturday", "2024-06-08")]
    [InlineData("sunday", "2024-06-09")]
    public void GivenADayOfWeekShouldReturnDateFromToday(string dayOfWeek, string expected)
    {
        // Arrange
        var today = new DateTime(2024, 06, 06);
        _timeProvider.SetUtcNow(today);
        var sut = new DateTimeConverter(_timeProvider);
        var exact = DateTime.ParseExact(expected, "yyyy-MM-dd", provider);

        // Act
        var date = sut.ConvertFrom(dayOfWeek);
        // Assert
        date.Should().Be(exact, "a synoymn should correctly convert to a date");
    }
}
