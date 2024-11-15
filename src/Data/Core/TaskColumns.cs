using TaskTitan.Core;
using TaskTitan.Core.Enums;

namespace TaskTitan.Data.Expressions;

// Define available columns as a static class with constants
public static class TaskColumns
{
    public static readonly string Id = nameof(TaskItem.Id).ToLower();
    public static readonly string Description = nameof(TaskItem.Description).ToLower();
    public static readonly string Due = nameof(TaskItem.Due).ToLower();
    public static readonly string End = nameof(TaskItem.End).ToLower();
    public static readonly string Entry = nameof(TaskItem.Entry).ToLower();
    public static readonly string Modified = nameof(TaskItem.Modified).ToLower();
    // public static readonly string Parent = nameof(TaskItem.Parent).ToLower();
    public static readonly string Project = nameof(TaskItem.Project).ToLower();
    public static readonly string Recur = nameof(TaskItem.Recur).ToLower();
    public static readonly string Scheduled = nameof(TaskItem.Scheduled).ToLower();
    public static readonly string Start = nameof(TaskItem.Start).ToLower();
    public static readonly string Status = nameof(TaskItem.Status).ToLower();
    public static readonly string Tags = nameof(TaskItem.Tags).ToLower();
    public static readonly string Until = nameof(TaskItem.Until).ToLower();
    public static readonly string Urgency = nameof(TaskItem.Urgency).ToLower();
    public static readonly string Wait = nameof(TaskItem.Wait).ToLower();
    public static readonly string TaskId = nameof(TaskItem.TaskId).ToLower();
    public static readonly string Depends = nameof(TaskItem.Depends).ToLower();

    // Create a lookup for column types
    public static readonly IReadOnlyDictionary<string, ColType> ColumnTypes = new Dictionary<string, ColType>(StringComparer.OrdinalIgnoreCase)
    {
        [Id] = ColType.Number,
        [Description] = ColType.Text,
        [Due] = ColType.Date,
        [End] = ColType.Date,
        [Entry] = ColType.Date,
        [Modified] = ColType.Date,
        // [Parent] = ColType.Text,
        [Project] = ColType.Text,
        [Scheduled] = ColType.Date,
        [Start] = ColType.Date,
        [Status] = ColType.Text,
        [Tags] = ColType.Text,
        [Until] = ColType.Date,
        [Urgency] = ColType.Number,
        [Wait] = ColType.Date,
        [TaskId] = ColType.Text
    };


    public static bool IsValidColumn(string columnName) =>
        ColumnTypes.ContainsKey(columnName);

    public static ColType? GetColumnType(string columnName) =>
        ColumnTypes.TryGetValue(columnName, out var type) ? type : null;

    internal static string GetColumnName(string field) =>
        ColumnTypes.Keys.FirstOrDefault(k => k.StartsWith(field)) ?? field;
}
