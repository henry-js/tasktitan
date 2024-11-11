using TaskTitan.Core.Enums;

namespace TaskTitan.Core;

public abstract record TaskAttribute : Expr
{
    public TaskAttribute(string name, ColModifier modifier)
    {
        Name = name; // Since validation is done at factory level, we can simplify this
        Modifier = modifier;
    }

    public string Name { get; }
    public ColModifier Modifier { get; }
    public AttributeKind AttributeKind { get; protected init; }
}

public record TaskAttribute<T> : TaskAttribute
{
    internal TaskAttribute(string field, T value, AttributeKind attributeKind, ColModifier modifier = ColModifier.NoModifier) : base(field, modifier)
    {
        if (modifier == ColModifier.Include || modifier == ColModifier.Exclude)
            throw new InvalidOperationException($"Modifier: {modifier} is only allowed in Tags");

        AttributeKind = attributeKind;
        Value = value;
    }
    public T Value { get; }
}

public record TaskTag : TaskAttribute
{
    public TaskTag(string name, ColModifier modifier) : base(name, modifier)
    {
        AttributeKind = AttributeKind.Tag;
    }
}

public enum AttributeKind { BuiltIn, UserDefined, Tag }
