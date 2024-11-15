using TaskTitan.Core.Configuration;
using TaskTitan.Core.Enums;

namespace TaskTitan.Core;

public class AttributeDefinition : IConfig
{
    public ColType Type { get; set; }
    public required string Name { get; set; }
    public required string? Label { get; set; }
}
