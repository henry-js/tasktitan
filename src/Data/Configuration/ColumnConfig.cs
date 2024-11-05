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

    public AttributeColumnConfig(string name, bool isModifiable, ColType type)
    {
        Name = name;
        IsModifiable = isModifiable;
        ColType = type;
    }
}
