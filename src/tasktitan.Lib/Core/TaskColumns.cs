using TaskTitan.Lib;
using TaskTitan.Lib.Enums;

namespace TaskTitan.Data.Expressions;

// Define available columns as a static class with constants
public static class TaskColumns
{
    public static readonly string Id = string.Empty;// = nameof(TaskItem.Id).ToLower();
    public static readonly string Description = string.Empty;// = nameof(TaskItem.Description).ToLower();
    public static readonly string Due = string.Empty;// = nameof(TaskItem.Due).ToLower();
    public static readonly string End = string.Empty;// = nameof(TaskItem.End).ToLower();
    public static readonly string Entry = string.Empty;// = nameof(TaskItem.Entry).ToLower();
    public static readonly string Modified = string.Empty;// = nameof(TaskItem.Modified).ToLower();
    // public static readonly string Parent = string.Empty;// = nameof(TaskItem.Parent).ToLower();
    public static readonly string Project = string.Empty;// = nameof(TaskItem.Project).ToLower();
    public static readonly string Recur = string.Empty;// = nameof(TaskItem.Recur).ToLower();
    public static readonly string Scheduled = string.Empty;// = nameof(TaskItem.Scheduled).ToLower();
    public static readonly string Start = string.Empty;// = nameof(TaskItem.Start).ToLower();
    public static readonly string Status = string.Empty;// = nameof(TaskItem.Status).ToLower();
    public static readonly string Tags = string.Empty;// = nameof(TaskItem.Tags).ToLower();
    public static readonly string Until = string.Empty;// = nameof(TaskItem.Until).ToLower();
    public static readonly string Urgency = string.Empty;// = nameof(TaskItem.Urgency).ToLower();
    public static readonly string Wait = string.Empty;// = nameof(TaskItem.Wait).ToLower();
    public static readonly string TaskId = string.Empty;// = nameof(TaskItem.TaskId).ToLower();
    public static readonly string Depends = string.Empty;// = nameof(TaskItem.Depends).ToLower();

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
