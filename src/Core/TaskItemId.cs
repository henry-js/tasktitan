using NanoidDotNet;

namespace TaskTitan.Core;

public readonly record struct TaskItemId(string Value)
{
    public static TaskItemId Empty => new(string.Empty);

    public static TaskItemId NewTaskId() => new(Nanoid.Generate(Nanoid.Alphabets.NoLookAlikes, 9));

    public override string ToString() => Value;
}
