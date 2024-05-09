using System.Globalization;

namespace TaskTitan.Lib.Services;

internal static class StringDateParser
{
    public static DateTime ToDayOfMonth(string? dueString)
    {
        throw new NotImplementedException();
    }

    public static bool IsDayOfMonth(string dueString)
    {
        int splitIndex = 0;
        foreach (var c in dueString)
        {
            if (char.IsDigit(c)) splitIndex++;
            else break;
        }
        Convert.ToInt32(dueString[0..splitIndex], CultureInfo.CurrentCulture);

        return true;
    }
}
