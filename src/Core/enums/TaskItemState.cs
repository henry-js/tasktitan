namespace TaskTitan.Core;

// public enum TaskItemState { Pending, Completed, Deleted, Waiting }

public record struct TaskItemState(string Value)
{
    public static TaskItemState[] Values => [Pending, Completed, Deleted, Waiting];
    public static TaskItemState Pending => "pending";
    public static TaskItemState Completed => "completed";
    public static TaskItemState Deleted => "deleted";
    public static TaskItemState Waiting => "waiting";

    public static implicit operator TaskItemState(string value)
    {
        return new TaskItemState(value);
    }

    public static implicit operator string(TaskItemState taskItemAttribute)
    {
        return taskItemAttribute.ToString()!;
    }

    public override readonly string ToString() => Value;
}
