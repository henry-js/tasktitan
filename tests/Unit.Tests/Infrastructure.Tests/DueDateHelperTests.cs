namespace TaskTitan.Infrastructure.Tests;

public class DueDateHelperTests
{
    private readonly CultureInfo provider = CultureInfo.InvariantCulture;
    private readonly FakeTimeProvider _timeProvider = new();
    public DueDateHelperTests()
    {
        var gmt = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        _timeProvider.SetLocalTimeZone(gmt);
        _timeProvider.SetUtcNow(new DateTime(2024, 06, 06, 12, 0, 0));
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
        var sut = new TaskDateConverter(_timeProvider);
        var taskDate = sut.ConvertFrom(input);

        var date = (DateTime?)taskDate;

        date.Should().NotBeNull("the default format for date strings is 'yyyy-MM-dd'");
        var dateParts = input.Split("-").Select(s => Convert.ToInt32(s)).ToList();
        dateParts.Should().HaveElementAt(0, date.Value.Year);
        dateParts.Should().HaveElementAt(1, date.Value.Month);
        dateParts.Should().HaveElementAt(2, date.Value.Day);
        date!.Value.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Theory]
    [InlineData("today", "2024-06-06", true)]
    [InlineData("yesterday", "2024-06-05", true)]
    [InlineData("tomorrow", "2024-06-07", true)]
    [InlineData("eom", "2024-06-30", true)]
    [InlineData("eoy", "2024-12-31", true)]
    public void GivenARelativeSynonymShouldReturnAValidDateOnly(string synonym, string expected, bool asDateOnly)
    {
        // Arrange
        var sut = new TaskDateConverter(_timeProvider);
        DateTime.TryParse(expected, provider, DateTimeStyles.AssumeUniversal, out var exact);
        var exactTaskDate = new TaskDate(exact, asDateOnly);
        // Act
        var date = sut.ConvertFrom(synonym);

        // Assert
        date.Should().Be(exactTaskDate, "a synoymn should correctly convert to a date");
        date!.Value.IsDateOnly.Should().Be(asDateOnly);
        date.Value.Kind.Should().Be(DateTimeKind.Utc);
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
        var sut = new TaskDateConverter(_timeProvider);
        var exact = DateTime.ParseExact(expected, "yyyy-MM-dd", provider);
        var exactTaskDate = new TaskDate(DateOnly.Parse(expected, CultureInfo.CurrentCulture));
        // Act
        var date = sut.ConvertFrom(dayOfWeek);
        // Assert
        date.Should().NotBeNull();
        date.Should().Be(exactTaskDate, "a synoymn should correctly convert to a date");
        date!.Value.Kind.Should().Be(DateTimeKind.Utc);
    }
}
