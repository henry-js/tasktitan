using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using static TaskTitan.Data.Enums;

namespace TaskTitan.Data;

public class AttributeColumnConfig
{
    [JsonIgnore]
    public string Name { get; set; }
    public bool IsModifiable { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ColFormat Format { get; private set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ColType ColType { get; set; }
    public IReadOnlyList<ColFormat>? AllowedFormats { get; }

    public AttributeColumnConfig(string name, bool isModifiable, ColFormat format, ColType type, List<ColFormat>? allowedFormats = null)
    {
        Name = name;
        IsModifiable = isModifiable;
        Format = format;
        ColType = type;
        AllowedFormats = allowedFormats ?? AttributeColumnFormats.AllowedFormats[type];

    }

    public void SetFormat(ColFormat format)
    {
        if (!AttributeColumnFormats.AllowedFormats.TryGetValue(ColType, out List<ColFormat>? allowed))
            throw new ArgumentException($"Unknown column type '{ColType}'");

        if (allowed.Contains(format))
        {
            Format = format;
        }
    }
}
