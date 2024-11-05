using System.Runtime.Serialization;
using TaskTitan.Data.Reports;


namespace TaskTitan.Configuration;

public class ReportConfiguration
{
    [DataMember(Name = "Report")]
    public ConfigDictionary<CustomReport> Report { get; set; } = [];
    public ConfigDictionary<UserDefinedAttributeConfig> UDAs { get; set; } = [];
}

public static class TaskColumns
{

}
