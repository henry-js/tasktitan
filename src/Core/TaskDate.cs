using System.Reflection.Metadata;

namespace TaskTitan.Core;

public readonly record struct TaskDate
{
    public const string DateFormat = "yyyy-MM-dd";
    public const string RoundTripFormat = "o";
    public readonly DateTimeKind Kind => Value.Kind;
    public readonly DateTime Value { get; init; }
    public readonly bool IsDateOnly { get; }

    public TaskDate(DateTime dateTime, bool asDateOnly = false)
    {
        if (dateTime == DateTime.MinValue) throw new ArgumentOutOfRangeException(nameof(dateTime), dateTime, "dateTime cannot be MinValue");
        if (dateTime == DateTime.MaxValue) throw new ArgumentOutOfRangeException(nameof(dateTime), dateTime, "dateTime cannot be MaxValue");
        if (dateTime.Kind == DateTimeKind.Unspecified)
            // throw new ArgumentException("dateTime.Kind should not be Unspecified", nameof(dateTime));
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        if (dateTime.Kind == DateTimeKind.Local)
            dateTime = dateTime.ToUniversalTime();
        IsDateOnly = asDateOnly;
        Value = dateTime;
    }
    public TaskDate(DateTime dateTime) : this(dateTime, false) { }

    public TaskDate(DateOnly dateOnly) : this(dateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), true) { }

    public string ToString(string? format) => format is null ? ToString() : Value.ToString(format);

    public override string ToString() => IsDateOnly
        ? Value.ToString(DateFormat)
        : Value.ToString();

    public static TaskDate FromDateOnly(DateOnly dateOnly)
    {
        return new(dateOnly);
    }

    public static implicit operator TaskDate(DateTime dateTime) => new(dateTime);
    public static implicit operator DateTime(TaskDate taskDate) => taskDate.Value;
}
