using shortid;
using shortid.Configuration;

namespace TaskTitan.Core;

public readonly record struct MyTaskId(string Value)
{
    private static readonly GenerationOptions Options = new(useNumbers: true, useSpecialCharacters: false, length: 8);

    public static MyTaskId Empty => new(string.Empty);

    public static MyTaskId NewTaskId() => new(ShortId.Generate(Options));
}
