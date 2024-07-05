namespace TaskTitan.Infrastructure.Dates;

public class TaskDateConverter(TimeProvider _timeProvider) : IStringFilterConverter<TaskDate>
{
    public DateTimeOffset Now { get; } = _timeProvider.GetUtcNow();
    private readonly DayOfWeek[] _daysOfWeek = Enum.GetValues<DayOfWeek>();

    private TaskDate NextDayOfWeek(string day)
    {
        var newNow = Now.AddDays(1);

        while (newNow.DayOfWeek != ToDayOfWeek(day))
        {
            newNow = newNow.AddDays(1);
        }
        return DateOnly.FromDateTime(newNow.Date);
    }

    private bool IsDayOfWeek(string input) =>
        _daysOfWeek.Any(day => string.Equals(day.ToString(), input, StringComparison.InvariantCultureIgnoreCase));

    private DayOfWeek ToDayOfWeek(string day) =>
        _daysOfWeek.Single(d => d.ToString().Equals(day, StringComparison.InvariantCultureIgnoreCase));

    public TaskDate? ConvertFrom(string? value)
    {
        TaskDate? dt = value switch
        {
            "eom" => DateOnly.FromDateTime(new DateTime(Now.Year, Now.Month, DateTime.DaysInMonth(Now.Year, Now.Month))),
            "today" => DateOnly.FromDateTime(Now.Date),
            "tomorrow" => DateOnly.FromDateTime(Now.Date.AddDays(1)),
            "yesterday" => DateOnly.FromDateTime(Now.Date.AddDays(-1)),
            "eoy" => DateOnly.FromDateTime(new DateTime(Now.Year, 12, 31)),
            "eod" => Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
            string day when IsDayOfWeek(day) => NextDayOfWeek(day),
            string date when DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out var parsedDate) => parsedDate,
            _ => null,
        };

        return dt is not null ? DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc) : null;
    }
}
