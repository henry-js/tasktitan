using System.Collections.Frozen;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TaskTitan.Lib.Dates;

public class DueDateConverter : TypeConverter
{
    readonly DateParser _dateParser = new(TimeProvider.System);

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType != typeof(string))
        {
            return false;
        }
        return true;
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is not string strValue) return base.ConvertFrom(context, culture, value);

        strValue = strValue.Trim().ToLower(CultureInfo.InvariantCulture).Split(':')[^1];

        if (_dateParser.IsRelative(strValue, out DateOnly? relativeDate)) return relativeDate;

        if (_dateParser.IsExactDate(strValue, out DateOnly? exactDate)) return exactDate;

        if (_dateParser.IsNextDate(strValue, out DateOnly? nextDate)) return nextDate;

        // Add more string parsing logic here if needed

        throw new ArgumentException("Unsupported date format.", nameof(value));
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof(string))
        {
            return true;
        }
        return base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is DateOnly dateOnly)
        {
            // Convert DateOnly to a string representation, e.g., "2024-05-19"
            return dateOnly.ToString("yyyy-MM-dd", culture);
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}

public class DateParser(TimeProvider _timeProvider)
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
                string day when IsDayOfWeek(day) => NextDayOfWeek(day),
                _ => null,
            };
        }
    }

    public bool IsExactDate(string strValue, out DateOnly? exactDate)
    {
        var isStringDate = DateOnly.TryParseExact(strValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate);

        if (isStringDate) exactDate = parsedDate;
        else exactDate = null;
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
