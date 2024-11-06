using TaskTitan.Core.Enums;

namespace TaskTitan.Data.Expressions;

// Define available columns as a static class with constants
public static class TaskColumns
{
    public const string Description = "description";
    public const string Due = "due";
    public const string End = "end";
    public const string Entry = "entry";
    public const string Modified = "modified";
    public const string Parent = "parent";
    public const string Project = "project";
    public const string Scheduled = "scheduled";
    public const string Start = "start";
    public const string Status = "status";
    public const string Tags = "tags";
    public const string Until = "until";
    public const string Urgency = "urgency";
    public const string Wait = "wait";
    public const string Uuid = "uuid";

    // Create a lookup for column types
    public static readonly IReadOnlyDictionary<string, ColType> ColumnTypes = new Dictionary<string, ColType>(StringComparer.OrdinalIgnoreCase)
    {
        [Description] = ColType.Text,
        [Due] = ColType.Date,
        [End] = ColType.Date,
        [Entry] = ColType.Date,
        [Modified] = ColType.Date,
        [Parent] = ColType.Text,
        [Project] = ColType.Text,
        [Scheduled] = ColType.Date,
        [Start] = ColType.Date,
        [Status] = ColType.Text,
        [Tags] = ColType.Text,
        [Until] = ColType.Date,
        [Urgency] = ColType.Number,
        [Wait] = ColType.Date,
        [Uuid] = ColType.Text
    };

    public static bool IsValidColumn(string columnName) =>
        ColumnTypes.ContainsKey(columnName);

    public static ColType? GetColumnType(string columnName) =>
        ColumnTypes.TryGetValue(columnName, out var type) ? type : null;
}
