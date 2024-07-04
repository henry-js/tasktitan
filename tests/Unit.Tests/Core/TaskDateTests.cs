using static FluentAssertions.FluentActions;

namespace Unit.Tests.Core;

public class TaskDateTests
{
    private readonly DateTime localMidSummerDay;
    public TaskDateTests()
    {
        FakeTimeProvider _timeProvider = new();

        _timeProvider.SetLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"));
        _timeProvider.SetUtcNow(new DateTime(2024, 06, 06, 12, 0, 0));
        localMidSummerDay = _timeProvider.GetLocalNow().LocalDateTime;
    }

    [Fact]
    public void Constructor_WithUtcDateTime_ShouldCreateInstance()
    {
        var expected = localMidSummerDay.ToUniversalTime().ToString("o");

        var taskDate = new TaskDate(localMidSummerDay.ToUniversalTime());

        taskDate.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Constructor_WithLocalDateTime_ShouldConvertToUtc()
    {
        var taskDate = new TaskDate(localMidSummerDay);

        taskDate.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Constructor_WithDateOnly_ShouldCreateInstance()
    {
        var dateOnly = DateOnly.FromDateTime(localMidSummerDay);
        var taskDate = new TaskDate(dateOnly);

        taskDate.IsDateOnly.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithMinValue_ShouldThrowArgumentOutOfRangeException()
    {
        Invoking(() => new TaskDate(DateTime.MinValue)).Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_WithMaxValue_ShouldThrowArgumentOutOfRangeException()
    {
        Invoking(() => new TaskDate(DateTime.MaxValue)).Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_WithUnspecifiedKind_ShouldThrowArgumentException()
    {
        var localDateTime = new DateTime(localMidSummerDay.Ticks, DateTimeKind.Unspecified);

        Invoking(() => new TaskDate(localDateTime)).Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void ToString_WithDefaultFormat_ShouldReturnCorrectString()
    {
        var taskDate = new TaskDate(localMidSummerDay);
        var utcString = localMidSummerDay.ToUniversalTime().ToString("o");

        taskDate.ToString().Should().Be(utcString);
    }

    [Fact]
    public void ToString_WithCustomFormat_ShouldReturnCorrectString()
    {
        var taskDate = new TaskDate(localMidSummerDay);
        var customFormatString = "06/06/2024 12:00:00";

        taskDate.ToString("dd/MM/yyyy HH:mm:ss").Should().Be(customFormatString);
    }

    [Fact]
    public void ImplicitConversion_FromDateTime_ShouldCreateTaskDate()
    {
        TaskDate taskDate = localMidSummerDay;

        taskDate.ToString("g").Should().Be("06/06/2024 12:00");
    }

    [Fact]
    public void ImplicitConversion_FromDateOnly_ShouldCreateTaskDate()
    {
        DateOnly dateOnly = DateOnly.FromDateTime(localMidSummerDay);
        TaskDate taskDate = dateOnly;

        taskDate.ToString().Should().Be("2024-06-06");
    }
}
