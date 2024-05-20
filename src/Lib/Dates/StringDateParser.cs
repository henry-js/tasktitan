using System.Linq;

namespace TaskTitan.Lib.Dates;

public class StringDateParser(TimeProvider _timeProvider)
{
    public DateTimeOffset Now { get; } = _timeProvider.GetLocalNow();
    private readonly DayOfWeek[] _daysOfWeek = Enum.GetValues<DayOfWeek>();

    private DateOnly NextDayOfWeek(string day)
    {
        var newNow = Now.AddDays(1);

        while (newNow.DayOfWeek != ToDayOfWeek(day))
        {
            newNow = newNow.AddDays(1);
        }
        return DateOnly.FromDateTime(newNow.Date);
    }

    private DayOfWeek ToDayOfWeek(string day)
    {
        return _daysOfWeek.Single(d => d.ToString().Equals(day, StringComparison.InvariantCultureIgnoreCase));
    }
    private bool IsDayOfWeek(string input) =>
        _daysOfWeek.Any(day => string.Equals(day.ToString(), input, StringComparison.InvariantCultureIgnoreCase));

    public bool IsRelative(string strValue, out DateOnly? relativeDate)
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

    public bool IsExactDate(string strValue, out DateOnly? exactDate)
    {
        var isStringDate = DateOnly.TryParseExact(strValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate);

        exactDate = isStringDate ? parsedDate : null;
        return exactDate is not null;
    }

    public bool IsNextDate(string strValue, out DateOnly? nextDate)
    {
        nextDate = strValue switch
        {
            string day when IsDayOfWeek(day) => NextDayOfWeek(day),
            _ => null,
        };
        return nextDate != null;
    }

}
