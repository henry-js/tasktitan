// using System.Globalization;

// namespace TaskTitan.Lib.Dates;
// public class DueDateHelper(TimeProvider _timeProvider)
// {
//     private readonly TimeProvider _timeProvider = _timeProvider;
//     private readonly DayOfWeek[] _daysOfWeek = Enum.GetValues<DayOfWeek>();

//     public DateOnly? DateStringToDate(string input)
//     {
//         if (string.IsNullOrEmpty(input)) return null;
//         var isStringDate = DateOnly.TryParseExact(input, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateOnly);
//         if (isStringDate) return dateOnly;
//         else
//         {
//             return DateSynonymToDate(input);
//         }
//     }
//     public DateTimeOffset Now { get; } = _timeProvider.GetLocalNow();

//     public DateOnly? DateSynonymToDate(string input)
//     {
//         return input switch
//         {
//             "eom" => EndOfMonth(),
//             "today" => Today(),
//             "tomorrow" => Tomorrow(),
//             "yesterday" => Yesterday(),
//             string day when IsDayOfWeek(day) => NextDayOfWeek(day),
//             "eoy" => EndOfYear(),
//             _ => null,
//         };
//     }

//     private DateOnly NextDayOfWeek(string day)
//     {
//         var newNow = Now.AddDays(1);

//         while (newNow.DayOfWeek != ToDayOfWeek(day))
//         {
//             newNow = newNow.AddDays(1);
//         }
//         return DateOnly.FromDateTime(newNow.Date);
//     }

//     private DayOfWeek ToDayOfWeek(string day)
//     {
//         return _daysOfWeek.Single(d => d.ToString().Equals(day, StringComparison.InvariantCultureIgnoreCase));
//     }

//     private DateOnly Yesterday() => DateOnly.FromDateTime(Now.Date.AddDays(-1));
//     private DateOnly Tomorrow() => DateOnly.FromDateTime(Now.Date.AddDays(1));
//     private DateOnly Today() => DateOnly.FromDateTime(Now.Date);

//     private bool IsDayOfWeek(string input) =>
//         _daysOfWeek.Any(day => string.Equals(day.ToString(), input, StringComparison.InvariantCultureIgnoreCase));

//     private DateOnly EndOfMonth() => new(Now.Year, Now.Month, DateTime.DaysInMonth(Now.Year, Now.Month));
//     private DateOnly EndOfYear() => new(Now.Year, 12, 31);
// }
