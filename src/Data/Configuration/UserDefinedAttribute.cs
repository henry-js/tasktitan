using TaskTitan.Data.Expressions;
using TaskTitan.Data.Reports;

using static TaskTitan.Data.Enums;

namespace TaskTitan.Configuration;

public class UserDefinedAttributeConfig : IConfig
{
    public ColType Type { get; set; }
    public required string Name { get; set; }
    public required string? Label { get; set; }
}
