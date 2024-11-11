using LightResults;

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

public record Tag : TaskAttribute
{

    private readonly TagInner _inner;

    private const string InvalidTagCharacters = "+-*/(<>^!%=~";

    public bool IsSynthetic => _inner is TagInner.Synthetic;
    public bool IsUser => _inner is TagInner.User;

    private Tag(TagInner inner, ColModifier modifier) : base(inner.ToString(), modifier)
    {
        AttributeKind = AttributeKind.Tag;
        _inner = inner;
    }

    public static Result<Tag> TryFrom(string value, ColModifier modifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.All(char.IsUpper))
        {
            return Enum.TryParse<SyntheticTag>(value, out var syntheticTag)
                ? (Result<Tag>)new Tag(new TagInner.Synthetic(syntheticTag), modifier)
                : Result<Tag>.Fail("abc");
        }

        // Validate first character
        var firstChar = value[0];
        if (char.IsWhiteSpace(firstChar) || char.IsDigit(firstChar) || InvalidTagCharacters.Contains(firstChar))
            return Result<Tag>.Fail($"Invalid first character for tag: {firstChar}");

        // Validate remaining characters
        if (value.Skip(1).Any(c => char.IsWhiteSpace(c) || c == ':' || InvalidTagCharacters.Contains(c)))
            return Result<Tag>.Fail($"Invalid character in tag: {value}");

        return new Tag(new TagInner.User(value), modifier);
    }

    private abstract record TagInner
    {
        public abstract string AsRef();

        public sealed record User(string Value) : TagInner
        {
            public override string ToString() => Value;
            public override string AsRef() => Value;
        }

        public sealed record Synthetic(SyntheticTag Tag) : TagInner
        {
            public override string ToString() => Tag.ToString();
            public override string AsRef() => Tag.ToString();
        }
    }

    public enum SyntheticTag
    {
        Waiting,
        Active,
        Pending,
        Completed,
        Deleted,
        Blocked,
        Unblocked,
        Blocking
    }
}
