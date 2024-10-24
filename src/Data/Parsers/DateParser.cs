using System.Globalization;
using System.Runtime.CompilerServices;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static Pidgin.Parser<string>;

namespace TaskTitan.Data.Parsers;

public class DateParser
{
    private readonly TimeProvider timeProvider;
    private readonly TimeSpan eod = new(23, 59, 59);
    private readonly TimeSpan sod = new(0, 0, 0);

    public DateParser(TimeProvider timeProvider)
    {
        this.timeProvider = timeProvider;
    }
    private DateTime Utc => timeProvider.GetUtcNow().UtcDateTime;
    private DateTime Local => timeProvider.GetLocalNow().DateTime;
    public DateTime Parse(string input)
    {
        if (DateTime.TryParse(input, CultureInfo.InvariantCulture, out var parsedDate))
        {
            return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
        }

        var dt = input.ToLower() switch
        {
            "now" => Local,
            "today" or "sod" => Local.Date,
            "eod" => Local.Date.Add(new TimeSpan(23, 59, 59)),
            "yesterday" => Local.Date.AddDays(-1),
            "tomorrow" => Local.Date.AddDays(1),
            "soy" => new DateTime(Local.Year, 1, 1),
            "eoy" => new DateTime(Local.Year, 12, 31).Add(eod),
            "som" => StartOfMonth(),
            "eom" => EndOfMonth(),
            "sow" => Prev(DayOfWeek.Monday),
            "eow" => Next(DayOfWeek.Sunday),
            string day when IsDayOfWeek(day) => NextDayOfWeek(day),
            string quantity when char.IsDigit(quantity[0]) => ToDateQuantity(quantity),
            _ => throw new SwitchExpressionException(input)
        };
        return DateTime.SpecifyKind(dt, DateTimeKind.Local);

        bool IsDayOfWeek(string day)
        {
            return day switch
            {
            ['m', 'o', 'n', ..] => true,
            ['t', 'u', 'e', ..] => true,
            ['w', 'e', 'd', ..] => true,
            ['t', 'h', 'u', ..] => true,
            ['f', 'r', 'i', ..] => true,
            ['s', 'a', 't', ..] => true,
            ['s', 'u', 'n', ..] => true,
                _ => false
            };
        }

        DateTime EndOfMonth()
            => new DateTime(Local.Year, Local.Month, DateTime.DaysInMonth(Local.Year, Local.Month)).Add(eod);
        DateTime StartOfMonth()
            => new DateTime(Local.Year, Local.Month, 1);

        DateTime Prev(DayOfWeek day)
        {
            if (Local.DayOfWeek == day) return Local.Date;
            var current = Local;
            while (current.DayOfWeek != day)
            {
                current = current.AddDays(-1);
            }
            return current.Date;
        }
        DateTime Next(DayOfWeek day)
        {
            if (Local.DayOfWeek == day) return Local.Date.Add(eod);
            var current = Local;
            while (current.DayOfWeek != day)
            {
                current = current.AddDays(1);
            }
            return current.Date.Add(eod);
        }

        DateTime NextDayOfWeek(string day)
        {
            var newNow = Local.Date.AddDays(1);

            while (newNow.DayOfWeek != ToDayOfWeek(day))
            {
                newNow = newNow.AddDays(1);
            }
            return newNow.Date;

            DayOfWeek ToDayOfWeek(string day) =>
                System.Enum.GetValues<DayOfWeek>().Single(d => d.ToString().StartsWith(day, StringComparison.InvariantCultureIgnoreCase));

        }
    }

    private DateTime ToDateQuantity(string quantityText)
    {
        var (quantity, period) = ParseDateQuantity(quantityText);

        return (quantity, period) switch
        {
            (_, 'd') => Local.Date.AddDays(quantity),
            (_, 'w') => Local.Date.AddDays(quantity * 7),
            (_, 'm') => Local.Date.AddMonths(quantity),
            _ => throw new SwitchExpressionException(),
        };
    }



    private static (int, char) ParseDateQuantity(string input)
        => Map(
            (a, b) => (Convert.ToInt32(a), b),
            Digit.AtLeastOnceString(),
            Token('w').Or(Token('d'))
            .Or(Token('m'))
        ).ParseOrThrow(input);
}
