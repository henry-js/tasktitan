using System.Runtime.CompilerServices;

using TaskTitan.Configuration;
using TaskTitan.Data.Parsers;

using static TaskTitan.Data.Enums;

namespace TaskTitan.Data.Expressions;

public abstract record TaskProperty : Expr
{
    public TaskProperty(string name, PropertyKind propertyKind, ColModifier? modifier)
    {
        Name = name;
        PropertyName = name; // Since validation is done at factory level, we can simplify this
        Modifier = modifier;
        PropertyKind = propertyKind;
    }

    public string Name { get; }
    public string PropertyName { get; }
    public ColModifier? Modifier { get; }
    public PropertyKind PropertyKind { get; init; }
}

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

// Factory class for creating TaskProperties
public static class TaskPropertyFactory
{
    public static TaskProperty Create(string input, string value, DateParser dateParser, Dictionary<string, UserDefinedAttributeConfig> udas)
    {
        udas ??= new Dictionary<string, UserDefinedAttributeConfig>();

        var (field, modifier) = ParseInput(input);

        // Try to get the column type
        var colType = TaskColumns.GetColumnType(field);
        if (!colType.HasValue)
        {
            // Handle UDA case here if needed
            udas.TryGetValue(field, out var uda);

            if (uda is not null)
            {
                return CreateProperty(uda, value, modifier, dateParser);
            }
            else
            {
                throw new ArgumentException($"Invalid column name: {field}");
            }
        }

        return CreateProperty(field, value, colType.Value, modifier, dateParser);
    }

    private static (string field, ColModifier? modifier) ParseInput(string input)
    {
        var split = input.Split('.');
        return split.Length switch
        {
            1 => (split[0], null),
            2 => (split[0], ParseModifier(split[1])),
            _ => throw new ArgumentException($"Invalid input format: {input}")
        };
    }

    private static ColModifier? ParseModifier(string modifierStr)
    {
        return Enum.GetValues<ColModifier>()
            .FirstOrDefault(m => m.ToString().Contains(modifierStr, StringComparison.OrdinalIgnoreCase));
    }

    private static TaskProperty CreateProperty(string field, string value, ColType colType, ColModifier? modifier, DateParser dateParser)
    {
        return colType switch
        {
            ColType.Date => new TaskProperty<DateTime>(field, dateParser.Parse(value), PropertyKind.BuiltInAttribute, modifier),
            ColType.Text => new TaskProperty<string>(field, value, PropertyKind.BuiltInAttribute, modifier),
            ColType.Number => new TaskProperty<double>(field, Convert.ToDouble(value), PropertyKind.BuiltInAttribute, modifier),
            _ => throw new ArgumentException($"Unsupported column type: {colType}")
        };
    }
    private static TaskProperty CreateProperty(UserDefinedAttributeConfig uda, string value, ColModifier? modifier, DateParser dateParser)
    {
        return uda.Type switch
        {
            ColType.Date => new TaskProperty<DateTime>(uda.Name, dateParser.Parse(value), PropertyKind.UserDefinedAttribute, modifier),
            ColType.Text => new TaskProperty<string>(uda.Name, value, PropertyKind.UserDefinedAttribute, modifier),
            ColType.Number => new TaskProperty<double>(uda.Name, Convert.ToDouble(value), PropertyKind.UserDefinedAttribute, modifier),
            _ => throw new ArgumentException($"Unsupported column type: {uda.Name}")
        };
    }
}

public record TaskProperty<T> : TaskProperty
{
    internal TaskProperty(string field, T value, PropertyKind propertyKind, ColModifier? modifier = null) : base(field, propertyKind, modifier)
    {
        Value = value;
    }
    public T Value { get; }
}
public record TaskTag : TaskProperty
{
    public TaskTag(string name, ColModifier modifier) : base(name, PropertyKind.Tag, modifier)
    {
    }
}

public enum PropertyKind { BuiltInAttribute, UserDefinedAttribute, Tag }
