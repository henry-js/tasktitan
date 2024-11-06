using System.Runtime.Serialization;

using TaskTitan.Data.Reports;

namespace TaskTitan.Core.Configuration;

public class TaskTitanConfig
{
    [DataMember(Name = "Report")]
    public ConfigDictionary<ReportDefinition> Report { get; set; } = [];
    public ConfigDictionary<AttributeDefinition> Uda { get; set; } = [];
}
