using static TaskTitan.Data.Enums;
using static TaskTitan.Data.Enums.ColType;
using static TaskTitan.Data.Enums.ColFormat;


namespace TaskTitan.Data;

public static class AttributeColumnFormats
{
    public static readonly Dictionary<ColType, List<ColFormat>> AllowedFormats = new()
    {
        { Date, new List<ColFormat> { Formatted, Julian, Epoch, Iso, Age, Relative, Remaining, Countdown } },
        { Text, new List<ColFormat> { Standard, Combined, Desc, Oneline, Truncated, Count, TruncatedCount } },
        { ColType.Number, new List<ColFormat> { ColFormat.Number, Real, Integer } }
    };
}
