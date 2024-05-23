namespace TaskTitan.Lib.Services;

public record TTaskResult(bool IsSuccess, TTask? task, params string[] ErrorMessages)
{
    public bool IsSuccess { get; set; } = IsSuccess;
    public TTask? Task { get; set; } = task;
    public string[] ErrorMessages { get; set; } = ErrorMessages ?? [];
    internal static TTaskResult Fail(string message)
    {
        return new TTaskResult(false, null, message);
    }

    internal static TTaskResult Success()
    {
        return new TTaskResult(true, null, []);
    }
}
