namespace TaskTitan.Lib.Dates;

public class DateOnlyConverter(TimeProvider _timeProvider) : IDateTimeConverter
{
    public DateTimeOffset Now { get; } = _timeProvider.GetLocalNow();
    private readonly DayOfWeek[] _daysOfWeek = Enum.GetValues<DayOfWeek>();


    private bool IsRelative(string strValue, out DateOnly? relativeDate)
    {
        relativeDate = RelativeToDate(strValue);
        return relativeDate != null;

        DateOnly? RelativeToDate(string input)
        {
            return input switch
            {
                "eom" => new(Now.Year, Now.Month, DateTime.DaysInMonth(Now.Year, Now.Month)),
                "today" => DateOnly.FromDateTime(Now.Date),
                "tomorrow" => DateOnly.FromDateTime(Now.Date.AddDays(1)),
                "yesterday" => DateOnly.FromDateTime(Now.Date.AddDays(-1)),
                "eoy" => new(Now.Year, 12, 31),
                _ => null,
            };
        }
    }
    private bool IsExactDate(string strValue, out DateOnly? exactDate)
    {
        var isStringDate = DateOnly.TryParseExact(strValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate);

        exactDate = isStringDate ? parsedDate : null;
        return exactDate is not null;
    }

    private bool IsNextDate(string strValue, out DateOnly? nextDate)
    {
        nextDate = strValue switch
        {
            string day when IsDayOfWeek(day) => NextDayOfWeek(day),
            _ => null,
        };
        return nextDate != null;
    }

    private DateOnly NextDayOfWeek(string day)
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

    public DateOnly? ConvertFrom(string value)
    {
        if (value == string.Empty) return null;

        if (IsExactDate(value, out DateOnly? date)) return date;
        if (IsNextDate(value, out date)) return date;
        if (IsRelative(value, out date)) return date;
        return null;
    }
}
