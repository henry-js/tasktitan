using System.Runtime.Serialization;

using static TaskTitan.Core.Enums.ColFormat;
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Reports;

namespace TaskTitan.Core.Configuration;

public class TaskTitanConfig
{
    [DataMember(Name = "Report")]
    public ConfigDictionary<ReportDefinition> Report { get; set; } = [];
    public ConfigDictionary<AttributeDefinition> Uda { get; set; } = [];
    public static ConfigDictionary<ColumnDefinition> DefinedColumns { get; } = new()
    {
        [TaskColumns.Id] = new ColumnDefinition(TaskColumns.Id, false, [Number]),
        [TaskColumns.Description] = new ColumnDefinition(TaskColumns.Description, false, [Desc, Oneline, Truncated, Count, TruncatedCount]),
        [TaskColumns.Due] = new ColumnDefinition(TaskColumns.Due, false, [Formatted, Epoch, Iso, Age, Relative, Remaining, Countdown]),
        [TaskColumns.End] = new ColumnDefinition(TaskColumns.End, false, [Formatted, Epoch, Iso, Age, Relative, Remaining, Countdown]),
        [TaskColumns.Entry] = new ColumnDefinition(TaskColumns.Entry, false, [Formatted, Epoch, Iso, Age, Relative, Remaining, Countdown]),
        [TaskColumns.Depends] = new ColumnDefinition(TaskColumns.Depends, true, [Indicator]),
        // [TaskColumns.Estimate] = new AttributeColumnConfig(TaskColumns.Estimate, true, []),
        [TaskColumns.Modified] = new ColumnDefinition(TaskColumns.Modified, false, [Formatted, Epoch, Iso, Age, Relative, Remaining, Countdown]),
        // [TaskColumns.Parent] = new AttributeColumnConfig(TaskColumns.Parent, true, []),
        [TaskColumns.Project] = new ColumnDefinition(TaskColumns.Project, false, [Full, Parent, Indented]),
        [TaskColumns.Recur] = new ColumnDefinition(TaskColumns.Recur, true, [Indicator, Duration]),
        [TaskColumns.Scheduled] = new ColumnDefinition(TaskColumns.Scheduled, false, [Formatted, Epoch, Iso, Age, Relative, Remaining, Countdown]),
        [TaskColumns.Start] = new ColumnDefinition(TaskColumns.Start, false, [Formatted, Epoch, Iso, Age, Relative, Remaining, Countdown, Active]),
        [TaskColumns.Status] = new ColumnDefinition(TaskColumns.Status, false, [Long, Short]),
        [TaskColumns.Tags] = new ColumnDefinition(TaskColumns.Tags, false, [List, Indicator, Count]),
        [TaskColumns.Until] = new ColumnDefinition(TaskColumns.Until, false, [Formatted, Epoch, Iso, Age, Relative, Remaining, Countdown]),
        [TaskColumns.Wait] = new ColumnDefinition(TaskColumns.Wait, false, [Formatted, Epoch, Iso, Age, Relative, Remaining, Countdown]),
        [TaskColumns.TaskId] = new ColumnDefinition(TaskColumns.TaskId, true, [Long, Short]),
        [TaskColumns.Urgency] = new ColumnDefinition(TaskColumns.Urgency, true, [Real, Number]),
    };
}
