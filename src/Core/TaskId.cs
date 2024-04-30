using shortid;
using shortid.Configuration;

namespace TaskTitan.Core;

public readonly record struct TaskId(string Value)
{
    private static readonly GenerationOptions Options = new(useNumbers: true, useSpecialCharacters: false, length: 8);

    public static TaskId Empty => new(string.Empty);

    public static TaskId NewTaskId() => new(ShortId.Generate(Options));
}
