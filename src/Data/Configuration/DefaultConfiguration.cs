using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using TaskTitan.Data;
using TaskTitan.Data.Reports;

using static TaskTitan.Data.Enums.ColFormat;
using static TaskTitan.Data.Enums.ColType;

namespace TaskTitan.Configuration;

public class ReportConfiguration
{
    [DataMember(Name = "Report")]
    public ConfigDictionary<CustomReport> Report { get; set; } = [];
    public ConfigDictionary<UserDefinedAttributeConfig> UDAs { get; set; } = [];
}

public static class TaskColumns
{
    public static readonly Dictionary<string, AttributeColumnConfig> Columns = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(TaskItem.Description)] = new AttributeColumnConfig(nameof(TaskItem.Description), true, Text),
        [nameof(TaskItem.Due)] = new AttributeColumnConfig(nameof(TaskItem.Due), true, Date),
        [nameof(TaskItem.End)] = new AttributeColumnConfig(nameof(TaskItem.End), true, Date),
        [nameof(TaskItem.Entry)] = new AttributeColumnConfig(nameof(TaskItem.Entry), true, Date),
        // [nameof(TaskItem.Estimate)] = new AttributeColumnConfig(nameof(TaskItem.Estimate), true, Text),
        [nameof(TaskItem.Modified)] = new AttributeColumnConfig(nameof(TaskItem.Modified), true, Date),
        // [nameof(TaskItem.Parent)] = new AttributeColumnConfig(nameof(TaskItem.Parent), false, Text),
        [nameof(TaskItem.Project)] = new AttributeColumnConfig(nameof(TaskItem.Project), false, Text),
        // [nameof(TaskItem.Recur)] = new AttributeColumnConfig(nameof(TaskItem.Recur), false, Text),
        [nameof(TaskItem.Scheduled)] = new AttributeColumnConfig(nameof(TaskItem.Scheduled), true, Date),
        [nameof(TaskItem.Start)] = new AttributeColumnConfig(nameof(TaskItem.Start), true, Date),
        [nameof(TaskItem.Status)] = new AttributeColumnConfig(nameof(TaskItem.Status), true, Text),
        [nameof(TaskItem.Tags)] = new AttributeColumnConfig(nameof(TaskItem.Tags), true, Text),
        [nameof(TaskItem.Until)] = new AttributeColumnConfig(nameof(TaskItem.Until), true, Date),
        [nameof(TaskItem.Wait)] = new AttributeColumnConfig(nameof(TaskItem.Wait), true, Date),
        [nameof(TaskItem.TaskId)] = new AttributeColumnConfig(nameof(TaskItem.TaskId), false, Text),
    };
}
