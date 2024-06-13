namespace TaskTitan.Core;

public readonly record struct TaskItemId(string Value)
{
    private static readonly GenerationOptions Options = new(useNumbers: true, useSpecialCharacters: false, length: 8);

    public static TaskItemId Empty => new(string.Empty);

    public static TaskItemId NewTaskId() => new(ShortId.Generate(Options));

    public override string ToString()
    {
        return Value;
    }
}
