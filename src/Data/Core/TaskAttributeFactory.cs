using TaskTitan.Core.Configuration;
using TaskTitan.Core.Enums;
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Parsers;
using TaskTitan.Data.Reports;

namespace TaskTitan.Core;

// Factory class for creating TaskProperties
public static class TaskAttributeFactory
{
    public static TaskAttribute Create(string input, string value, DateParser dateParser, ConfigDictionary<AttributeDefinition> udas)
    {
        udas ??= [];

        var (field, modifier) = ParseInput(input);

        // Try to get the column type
        var colType = TaskColumns.GetColumnType(field);
        if (!colType.HasValue)
        {
            // Handle UDA case here if needed
            udas.TryGetValue(field, out var uda);

            if (uda is not null)
            {
                return CreateAttribute(uda, value, modifier, dateParser);
            }
            else
            {
                throw new ArgumentException($"Invalid column name: {field}");
            }
        }

        return CreateAttribute(field, value, colType.Value, modifier, dateParser);
    }

    private static (string field, ColModifier modifier) ParseInput(string input)
    {
        var split = input.Split('.');
        return split.Length switch
        {
            1 => (TaskColumns.GetColumnName(split[0]), default),
            2 => (TaskColumns.GetColumnName(split[0]), ParseModifier(split[1])),
            _ => throw new ArgumentException($"Invalid input format: {input}")
        };
    }

    private static ColModifier ParseModifier(string modifierStr)
    {
        return Enum.GetValues<ColModifier>()
            .FirstOrDefault(m => m.ToString().Contains(modifierStr, StringComparison.OrdinalIgnoreCase));
    }

    private static TaskAttribute CreateAttribute(string field, string value, ColType colType, ColModifier modifier, DateParser dateParser)
    {
        if (field == TaskColumns.Description && string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A task must have a description.");
        return colType switch
        {
            ColType.Date => new TaskAttribute<DateTime>(field, dateParser.Parse(value), AttributeKind.BuiltIn, modifier),
            ColType.Text => new TaskAttribute<string>(field, value, AttributeKind.BuiltIn, modifier),
            ColType.Number => new TaskAttribute<double>(field, Convert.ToDouble(value), AttributeKind.BuiltIn, modifier),
            _ => throw new ArgumentException($"Unsupported column type: {colType}")
        };
    }
    private static TaskAttribute CreateAttribute(AttributeDefinition uda, string value, ColModifier modifier, DateParser dateParser)
    {
        return uda.Type switch
        {
            ColType.Date => new TaskAttribute<DateTime>(uda.Name, dateParser.Parse(value), AttributeKind.UserDefined, modifier),
            ColType.Text => new TaskAttribute<string>(uda.Name, value, AttributeKind.UserDefined, modifier),
            ColType.Number => new TaskAttribute<double>(uda.Name, Convert.ToDouble(value), AttributeKind.UserDefined, modifier),
            _ => throw new ArgumentException($"Unsupported column type: {uda.Name}")
        };
    }

    public static TaskAttribute CreateBuiltIn<T>(string field, T value)
    {
        if (!TaskColumns.IsValidColumn(field)) throw new Exception($"{field} is not a  built in column");

        return value switch
        {
            DateTime dt => new TaskAttribute<DateTime>(field, dt, AttributeKind.BuiltIn),
            string s => new TaskAttribute<string>(field, s, AttributeKind.BuiltIn),
            double num => new TaskAttribute<double>(field, num, AttributeKind.BuiltIn),
            _ => throw new ArgumentException($"Unsupported column type: {typeof(T)}")
        };
    }
}
