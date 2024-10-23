using System.Globalization;
using Microsoft.Extensions.Time.Testing;
using TaskTitan.Data.Parsers;

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
    [Test]
    [Arguments("2024-01-01")]
    [Arguments("2024-04-16")]
    [Arguments("2024-05-11")]
    [Arguments("2024-08-26")]
    [Arguments("2024-09-22")]
    [Arguments("2024-11-04")]
    public async Task GivenAStringDateShouldReturnAValidDateOnly(string input)
    {
        var sut = new DateParser(_timeProvider);
        var taskDate = sut.Parse(input);

        var date = (DateTime?)taskDate;

        await Assert.That(date).IsNotNull();
        var dateParts = input.Split("-").Select(s => Convert.ToInt32(s)).ToList();
        await Assert.That(dateParts[0]).IsEqualTo(date.Value.Year);
        await Assert.That(dateParts[1]).IsEqualTo(date.Value.Month);
        await Assert.That(dateParts[2]).IsEqualTo(date.Value.Day);
        await Assert.That(date!.Value.Kind).IsEquatableOrEqualTo(DateTimeKind.Utc);
    }

    [Test]
    [Arguments("today", "2024-06-06")]
    [Arguments("yesterday", "2024-06-05")]
    [Arguments("tomorrow", "2024-06-07")]
    [Arguments("eom", "2024-06-30 23:59:59")]
    [Arguments("eoy", "2024-12-31 23:59:59")]
    [Arguments("eow", "2024-06-09 23:59:59")]
    [Arguments("sow", "2024-06-03 00:00:00")]
    public async Task GivenARelativeSynonymShouldReturnAValidDateOnly(string synonym, string expectedString)
    {
        // Arrange
        var sut = new DateParser(_timeProvider);
        var expected = DateTime.Parse(expectedString, provider, DateTimeStyles.AssumeLocal);
        // Act
        var date = sut.Parse(synonym);

        // Assert
        await Assert.That(date).IsEqualTo(expected);
        await Assert.That(date.Kind).IsEquatableOrEqualTo(DateTimeKind.Local);
    }

    [Test]
    [Arguments("monday", "2024-06-10")]
    [Arguments("tuesday", "2024-06-11")]
    [Arguments("wednesday", "2024-06-12")]
    [Arguments("thursday", "2024-06-13")]
    [Arguments("friday", "2024-06-07")]
    [Arguments("saturday", "2024-06-08")]
    [Arguments("sunday", "2024-06-09")]
    public async Task GivenADayOfWeekShouldReturnDateFromToday(string dayOfWeek, string expectedString)
    {
        // Arrange
        var sut = new DateParser(_timeProvider);
        var expected = DateTime.Parse(expectedString, provider);
        // Act
        var date = sut.Parse(dayOfWeek);
        // Assert
        await Assert.That(date).IsNotNull();
        await Assert.That(date!).IsEquatableOrEqualTo(expected);
    }
}
