using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

using TaskTitan.Core.Configuration;
using TaskTitan.Core.Enums;

using static TaskTitan.Core.Enums.ColFormat;

namespace TaskTitan.Core;

public class ColumnDefinition : IConfig
{
    private readonly Dictionary<ColFormat, Func<object, string>> FormatFunctions;

    [JsonIgnore]
    public string Name { get; set; }
    public bool ReadOnly { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ColFormat Format { get; private set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public IReadOnlyList<ColFormat> AllowedFormats { get; } = [];
    public Func<object, string> FormatFunc { get; private set; } = obj => obj.ToString() ?? "";
    public bool IsUda { get; }

    private ColumnDefinition(string name)
    {
        Name = name;
        FormatFunctions = new()
        {
            [Default] = (val) => val?.ToString() ?? string.Empty,
            [Formatted] = (val) => val is DateTime date ? date.ToString("yyyy-MM-dd") : "",
            [Epoch] = (val) => val is DateTime date ? new DateTimeOffset(date).ToUnixTimeSeconds().ToString() : "",
            [Iso] = (val) => val is DateTime date ? date.ToString("yyyy-MM-ddTHH:mm:ssZ") : "",
            [Age] = (val) => val is DateTime date ? (DateTime.Now - date).ToString() : "",
            [Number] = val => val is double num ? num.ToString("N0") : val is int intNum ? intNum.ToString() : "",
            [Real] = val => val is double num ? num.ToString("N3") : val is int intNum ? intNum.ToString() : "",
            [Indicator] = val => name[0].ToString().ToUpper(),
            [Desc] = val => val is string s ? s.ToString() : string.Empty,
            [Full] = val => val is string s ? s.ToString() : string.Empty,
            [Indented] = val => $"\t{val}" ?? string.Empty,
            [Parent] = val => val is string s ? s.ToString().Split('.')[^2] : "",
            [List] = val => val is object[] arr ? $"[{string.Join(" ", arr)}]" : "[]",
            [Count] = val => val is object[] arr ? arr.Length.ToString() : "0",
            [Relative] = (val) => val is DateTime date ? date.ToString() : "",
            [Remaining] = (val) => val is DateTime date ? date.ToString() : "",
            [Countdown] = (val) => val is DateTime date ? date.ToString() : "",
        };
    }
    public ColumnDefinition(string name, bool readOnly, IEnumerable<ColFormat> allowedFormats) : this(name)
    {
        ReadOnly = readOnly;
        AllowedFormats = allowedFormats.ToImmutableArray();
    }

    public ColumnDefinition(AttributeDefinition attribute, bool isUda) : this(attribute.Name)
    {
        ReadOnly = false;
        AllowedFormats = attribute.Type switch
        {
            ColType.Text => [Default],
            ColType.Date => [Formatted, Epoch, Iso, Age, Relative, Remaining, Countdown],
            ColType.Number => [Default],
            _ => throw new SwitchExpressionException()
        };
        Format = AllowedFormats[0];
        FormatFunc = FormatFunctions[Format];
        IsUda = isUda;
    }

    public ColumnDefinition SetFormat(ColFormat format)
    {
        if (AllowedFormats.Contains(format)) Format = format;
        else Format = AllowedFormats[0];
        FormatFunc = FormatFunctions[Format];
        return this;
    }
}
