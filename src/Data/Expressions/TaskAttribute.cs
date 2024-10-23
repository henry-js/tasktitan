using TaskTitan.Data.Parsers;
using static TaskTitan.Data.Enums;

namespace TaskTitan.Data.Expressions;

public abstract record TaskProperty : Expr
{
    public TaskProperty(string name, ColModifier? modifier)
    {
        Name = name;
        Modifier = modifier;
    }

    public string Name { get; }
    public string PropertyName => _taskItemProperties.SingleOrDefault(p => p.Equals(Name, StringComparison.OrdinalIgnoreCase)) ?? Name;
    public ColModifier? Modifier { get; }

    public static TaskProperty Create(string field, string value, DateParser _dateParser)
    {
        var split = field.Split('.');
        field = split[0];
        var colKey = Configuration.ReportConfiguration.Columns.Keys
            .FirstOrDefault(k => k.StartsWith(field, StringComparison.OrdinalIgnoreCase))
            ?? field;
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
                return new TaskAttribute<DateTime>(split[0], dateVal, modifier);
            case ColType.Text:
                return new TaskAttribute<string>(split[0], value, modifier);
            case ColType.Number:
                return new TaskAttribute<double>(split[0], Convert.ToDouble(value), modifier);
            default:
                throw new Exception();
        }
    }

    private static readonly string[] _taskItemProperties = typeof(TaskItem).GetProperties().Select(x => x.Name).ToArray();
}

public record TaskAttribute<T> : TaskProperty
{
    internal TaskAttribute(string field, T value, ColModifier? modifier = null) : base(field, modifier)
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
