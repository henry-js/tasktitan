using TaskTitan.Lib.Enums;

using static TaskTitan.Lib.Enums.ColFormat;
using static TaskTitan.Lib.Enums.ColType;


namespace TaskTitan.Lib;

public static class ReportColumnFormats
{
    public static readonly Dictionary<ColType, List<ColFormat>> AllowedFormats = new()
    {
        { Date, new List<ColFormat> { Formatted, Julian, Epoch, Iso, Age, Relative, Remaining, Countdown } },
        { Text, new List<ColFormat> { Standard, Combined, Desc, Oneline, Truncated, Count, TruncatedCount } },
        { ColType.Number, new List<ColFormat> { ColFormat.Number, Real, Integer } }
    };
}
