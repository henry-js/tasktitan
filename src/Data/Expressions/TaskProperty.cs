using System.Runtime.CompilerServices;

using TaskTitan.Data.Parsers;

using static TaskTitan.Data.Enums;

namespace TaskTitan.Data.Expressions;

public abstract record TaskProperty : Expr
{
    private static readonly string[] _taskItemProperties = typeof(TaskItem).GetProperties().Select(x => x.Name).ToArray();

    public TaskProperty(string name, ColModifier? modifier)
    {
        Name = name;
        Modifier = modifier;
    }

    public string Name { get; }
    public string PropertyName => _taskItemProperties.SingleOrDefault(p => p.Equals(Name, StringComparison.OrdinalIgnoreCase)) ?? Name;
    public ColModifier? Modifier { get; }

    public static TaskProperty Create(string input, string value, DateParser _dateParser)
    {
        var split = input.Split('.');
        (string field, string? modifier) items = split switch
        {
            { Length: 1 } => (split[0], null),
            { Length: 2 } => (split[0], split[1]),
            _ => throw new SwitchExpressionException($"Invalid input: {input}"),
        };

        var colKey = Configuration.ReportConfiguration.Columns.Keys
            .FirstOrDefault(k => k.StartsWith(items.field, StringComparison.OrdinalIgnoreCase))
            ?? items.field;
        if (!Configuration.ReportConfiguration.Columns.TryGetValue(colKey, out var col))
        {
            //TODO: col is UDA
            // Configuration.UserDefinedAttributes.TryGetValue(colKey, out col);
        }

        ColModifier? modifier = null;

        if (split.Length < 2) modifier = null;
        // else modifier = col?.AllowedModifiers.FirstOrDefault(m => m.ToString().Equals(split[1], StringComparison.OrdinalIgnoreCase));
        else modifier = Enum.GetValues<ColModifier>().FirstOrDefault(m => m.ToString().Contains(split[1], StringComparison.OrdinalIgnoreCase));

        if (col is null) throw new Exception();
        switch (col.ColType)
        {
            case ColType.Date:
                var dateVal = _dateParser.Parse(value);
                return new TaskProperty<DateTime>(split[0], dateVal, modifier);
            case ColType.Text:
                return new TaskProperty<string>(split[0], value, modifier);
            case ColType.Number:
                return new TaskProperty<double>(split[0], Convert.ToDouble(value), modifier);
            default:
                throw new Exception();
        }
    }
}

public record TaskProperty<T> : TaskProperty
{
    internal TaskProperty(string field, T value, ColModifier? modifier = null) : base(field, modifier)
    {
        Value = value;
    }
    public T Value { get; }
}

public record TaskTag : TaskProperty
{
    public TaskTag(string name, ColModifier modifier) : base(name, modifier)
    {
    }
}
