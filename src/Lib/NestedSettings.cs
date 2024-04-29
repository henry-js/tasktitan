namespace TaskTitan.Lib;

public class NestedSettings
{
    public const string Key = nameof(NestedSettings);
    public string Item3 { get; set; } = default!;
    public Deeper Deeper { get; set; } = default!;
}
