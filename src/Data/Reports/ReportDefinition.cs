using System.Text.Json.Serialization;

using TaskTitan.Core.Configuration;

namespace TaskTitan.Data.Reports;

public class ReportDefinition : IConfig
{
    [JsonIgnore]
    public string Name { get; set; } = default!;
    public required string Description { get; set; }
    public string Filter { get; set; } = string.Empty;
    public string[] Columns { get; set; } = [];
    public string[] Labels { get; set; } = [];

    public static ReportDefinition FromFilter(string v)
    {
        throw new NotImplementedException();
    }

    public ReportDefinition OverrideFilter(params string[] filter)
    {
        Filter = string.Join(' ', filter);

        return this;
    }

    // TODO: Add support for sorting
}
