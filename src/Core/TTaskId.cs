namespace TaskTitan.Core;

public readonly record struct TTaskId(string Value)
{
    private static readonly GenerationOptions Options = new(useNumbers: true, useSpecialCharacters: false, length: 8);

    public static TTaskId Empty => new(string.Empty);

    public static TTaskId NewTaskId() => new(ShortId.Generate(Options));
}
