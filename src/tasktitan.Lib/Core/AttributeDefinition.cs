using TaskTitan.Lib.Configuration;
using TaskTitan.Lib.Enums;

namespace TaskTitan.Lib;

public class AttributeDefinition : IConfig
{
    public ColType Type { get; set; }
    public required string Name { get; set; }
    public required string? Label { get; set; }
}
