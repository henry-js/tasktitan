namespace TaskTitan.Core;

public readonly record struct TaskDate
{
    public readonly DateTimeKind Kind => Value.Kind;
    private readonly DateTime Value { get; init; }
    public bool IsDateOnly { get; }

    private TaskDate(DateTime dateTime, bool asDateOnly = false)
    {
        if (dateTime == DateTime.MinValue) throw new ArgumentOutOfRangeException(nameof(dateTime), dateTime, "dateTime cannot be MinValue");
        if (dateTime == DateTime.MaxValue) throw new ArgumentOutOfRangeException(nameof(dateTime), dateTime, "dateTime cannot be MaxValue");
        if (dateTime.Kind == DateTimeKind.Unspecified) throw new ArgumentException("dateTime.Kind should not be Unspecified", nameof(dateTime));

        IsDateOnly = asDateOnly;
        Value = dateTime.ToUniversalTime();
    }
    public TaskDate(DateTime dateTime) : this(dateTime, false) { }

    public TaskDate(DateOnly dateOnly) : this(dateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), true) { }

    public string ToString(string? format) => format is null ? ToString() : Value.ToString(format);

    public override string ToString() => IsDateOnly
        ? Value.ToString("yyyy-MM-dd")
        : Value.ToString("o");

    public static implicit operator TaskDate(DateTime dateTime) => new(dateTime);
    public static implicit operator TaskDate(DateOnly dateOnly) => new(dateOnly);
    public static implicit operator DateTime(TaskDate taskDate) => taskDate.Value;
}
