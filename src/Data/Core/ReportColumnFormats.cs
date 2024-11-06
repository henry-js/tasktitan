using static TaskTitan.Core.Enums.ColType;
using static TaskTitan.Core.Enums.ColFormat;
using TaskTitan.Core.Enums;


namespace TaskTitan.Core;

public static class ReportColumnFormats
{
    public static readonly Dictionary<ColType, List<ColFormat>> AllowedFormats = new()
    {
        { Date, new List<ColFormat> { Formatted, Julian, Epoch, Iso, Age, Relative, Remaining, Countdown } },
        { Text, new List<ColFormat> { Standard, Combined, Desc, Oneline, Truncated, Count, TruncatedCount } },
        { ColType.Number, new List<ColFormat> { ColFormat.Number, Real, Integer } }
    };
}
