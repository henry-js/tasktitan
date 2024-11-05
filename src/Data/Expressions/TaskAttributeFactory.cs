using TaskTitan.Configuration;
using TaskTitan.Data.Parsers;
using TaskTitan.Data.Reports;

using static TaskTitan.Data.Enums;

namespace TaskTitan.Data.Expressions;

// Factory class for creating TaskProperties
public static class TaskAttributeFactory
{
    public static TaskAttribute Create(string input, string value, DateParser dateParser, ConfigDictionary<UserDefinedAttributeConfig> udas)
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

    private static TaskAttribute CreateAttribute(string field, string value, ColType colType, ColModifier? modifier, DateParser dateParser)
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
    private static TaskAttribute CreateAttribute(UserDefinedAttributeConfig uda, string value, ColModifier? modifier, DateParser dateParser)
    {
        return uda.Type switch
        {
            ColType.Date => new TaskAttribute<DateTime>(uda.Name, dateParser.Parse(value), AttributeKind.UserDefined, modifier),
            ColType.Text => new TaskAttribute<string>(uda.Name, value, AttributeKind.UserDefined, modifier),
            ColType.Number => new TaskAttribute<double>(uda.Name, Convert.ToDouble(value), AttributeKind.UserDefined, modifier),
            _ => throw new ArgumentException($"Unsupported column type: {uda.Name}")
        };
    }
}
