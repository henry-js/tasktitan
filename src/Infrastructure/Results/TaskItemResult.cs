namespace TaskTitan.Infrastructure.Services;

public record TaskItemResult(bool IsSuccess, TaskItem? task, params string[] ErrorMessages)
{
    public bool IsSuccess { get; set; } = IsSuccess;
    public TaskItem? Task { get; set; } = task;
    public string[] ErrorMessages { get; set; } = ErrorMessages ?? [];
    internal static TaskItemResult Fail(string message)
    {
        return new TaskItemResult(false, null, message);
    }

    internal static TaskItemResult Success()
    {
        return new TaskItemResult(true, null, []);
    }
}
