namespace TaskTitan.Lib.Dates;

public class DateTimeConverter(TimeProvider _timeProvider) : IStringFilterConverter<DateTime>
{
    public DateTimeOffset Now { get; } = _timeProvider.GetLocalNow();
    private readonly DayOfWeek[] _daysOfWeek = Enum.GetValues<DayOfWeek>();

    private bool IsRelative(string strValue, out DateTime? relativeDate)
    {
        relativeDate = RelativeToDate(strValue);
        return relativeDate != null;

        DateTime? RelativeToDate(string input)
        {
            return input switch
            {
                "eom" => new(Now.Year, Now.Month, DateTime.DaysInMonth(Now.Year, Now.Month)),
                "today" => Now.Date,
                "tomorrow" => Now.Date.AddDays(1),
                "yesterday" => Now.Date.AddDays(-1),
                "eoy" => new(Now.Year, 12, 31),
                _ => null,
            };
        }
    }
    private bool IsExactDate(string strValue, out DateTime? exactDate)
    {
        var isStringDate = DateTime.TryParseExact(strValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate);

        exactDate = isStringDate ? parsedDate : null;
        return exactDate is not null;
    }

    private bool IsNextDate(string strValue, out DateTime? nextDate)
    {
        nextDate = strValue switch
        {
            string day when IsDayOfWeek(day) => NextDayOfWeek(day),
            _ => null,
        };
        return nextDate != null;
    }

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
        DateTime? dt = null;
        if (string.IsNullOrEmpty(value)) return null;

        if (IsExactDate(value, out DateTime? date)) dt = date;
        if (IsNextDate(value, out date)) dt = date;
        if (IsRelative(value, out date)) dt = date;
        if (dt is null) return null;
        return dt.Value.ToUniversalTime();
    }
}
