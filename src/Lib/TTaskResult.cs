namespace TaskTitan.Lib.Services;

public record TTaskResult(bool IsSuccess, TTask? task, string? Messaqge = null)
{
    internal static TTaskResult Fail(string message)
    {
        return new TTaskResult(false, null, message);
    }

    internal static TTaskResult Success()
    {
        return new TTaskResult(true, null, null);
    }
}
