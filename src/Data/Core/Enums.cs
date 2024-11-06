using System.Text.Json.Serialization;

namespace TaskTitan.Core.Enums;

public enum ColType
{
    Text,
    Date,
    Number
}
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ColFormat
{
    Formatted,
    Julian,
    Epoch,
    Iso,
    Age,
    Relative,
    Remaining,
    Countdown,
    Standard,
    Combined,
    Desc,
    Oneline,
    Truncated,
    Count,
    TruncatedCount,
    Number,
    Real,
    Integer,
    List,
    Indicator,
    Long,
    Short,
    Full,
    Parent,
    Indented,
    Duration
}

public enum ColModifier
{
    NoModifier,
    None,
    Not,
    Before, Below,
    After, Above,
    Any,
    Is, Equals,
    Isnt,
    Has, Contains,
    Hasnt,
    Startswith, Left,
    Endswith, Right,
    Word,
    Noword,
    Include, Exclude
}
