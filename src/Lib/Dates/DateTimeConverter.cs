namespace TaskTitan.Lib.Dates;

public class DateTimeConverter(TimeProvider _timeProvider) : IStringFilterConverter<DateTime>
{
    public DateTimeOffset Now { get; } = _timeProvider.GetLocalNow();
    private readonly DayOfWeek[] _daysOfWeek = Enum.GetValues<DayOfWeek>();

    private DateTime NextDayOfWeek(string day)
    {
        var newNow = Now.AddDays(1);

        while (newNow.DayOfWeek != ToDayOfWeek(day))
        {
            newNow = newNow.AddDays(1);
        }
        return newNow.Date;
    }

    private bool IsDayOfWeek(string input) =>
        _daysOfWeek.Any(day => string.Equals(day.ToString(), input, StringComparison.InvariantCultureIgnoreCase));

    private DayOfWeek ToDayOfWeek(string day) =>
        _daysOfWeek.Single(d => d.ToString().Equals(day, StringComparison.InvariantCultureIgnoreCase));

    public DateTime? ConvertFrom(string? value)
    {
        DateTime? dt = value switch
        {
            "eom" => new(Now.Year, Now.Month, DateTime.DaysInMonth(Now.Year, Now.Month)),
            "today" => Now.Date,
            "tomorrow" => Now.Date.AddDays(1),
            "yesterday" => Now.Date.AddDays(-1),
            "eoy" => new(Now.Year, 12, 31),
            "eod" => Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
            string day when IsDayOfWeek(day) => NextDayOfWeek(day),
            string date when DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate) => parsedDate,
            _ => null,
        };

        return dt is not null ? DateTime.SpecifyKind(dt.Value, DateTimeKind.Local) : null;
    }
}
