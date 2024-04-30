using shortid;
using shortid.Configuration;

namespace TaskTitan.Domain;

public readonly record struct TaskId(string Value)
{
    private static readonly GenerationOptions options = new(useNumbers: true, useSpecialCharacters: false, length: 8);

    public static TaskId Empty => new(string.Empty);

    public static TaskId NewTaskId() => new(ShortId.Generate(options));
}
