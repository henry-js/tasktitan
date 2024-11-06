using System.Text.Json.Serialization;

using TaskTitan.Core.Enums;

using static TaskTitan.Core.Enums.ColType;

namespace TaskTitan.Core;

public class ReportColumn
{
    [JsonIgnore]
    public string Name { get; set; }
    public bool IsModifiable { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ColFormat Format { get; private set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ColType ColType { get; set; }
    public IReadOnlyList<ColFormat>? AllowedFormats { get; }

    public ReportColumn(string name, bool isModifiable, ColType type)
    {
        Name = name;
        IsModifiable = isModifiable;
        ColType = type;
    }

    public static readonly Dictionary<string, ReportColumn> Default = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(TaskItem.Description)] = new ReportColumn(nameof(TaskItem.Description), true, Text),
        [nameof(TaskItem.Due)] = new ReportColumn(nameof(TaskItem.Due), true, Date),
        [nameof(TaskItem.End)] = new ReportColumn(nameof(TaskItem.End), true, Date),
        [nameof(TaskItem.Entry)] = new ReportColumn(nameof(TaskItem.Entry), true, Date),
        // [nameof(TaskItem.Estimate)] = new AttributeColumnConfig(nameof(TaskItem.Estimate), true, Text),
        [nameof(TaskItem.Modified)] = new ReportColumn(nameof(TaskItem.Modified), true, Date),
        // [nameof(TaskItem.Parent)] = new AttributeColumnConfig(nameof(TaskItem.Parent), false, Text),
        [nameof(TaskItem.Project)] = new ReportColumn(nameof(TaskItem.Project), false, Text),
        // [nameof(TaskItem.Recur)] = new AttributeColumnConfig(nameof(TaskItem.Recur), false, Text),
        [nameof(TaskItem.Scheduled)] = new ReportColumn(nameof(TaskItem.Scheduled), true, Date),
        [nameof(TaskItem.Start)] = new ReportColumn(nameof(TaskItem.Start), true, Date),
        [nameof(TaskItem.Status)] = new ReportColumn(nameof(TaskItem.Status), true, Text),
        [nameof(TaskItem.Tags)] = new ReportColumn(nameof(TaskItem.Tags), true, Text),
        [nameof(TaskItem.Until)] = new ReportColumn(nameof(TaskItem.Until), true, Date),
        [nameof(TaskItem.Wait)] = new ReportColumn(nameof(TaskItem.Wait), true, Date),
        [nameof(TaskItem.TaskId)] = new ReportColumn(nameof(TaskItem.TaskId), false, Text),
    };
}
