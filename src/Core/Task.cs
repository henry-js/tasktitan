namespace TaskTitan.Core;

public class MyTask
{
    private MyTask() { }
    public MyTask(string description)
    {
        Description = description;
    }

    public MyTaskId Id { get; private set; } = MyTaskId.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public TaskState State { get; private set; }

    public static MyTask CreateNew(string description)
    {
        MyTask task = new()
        {
            Id = MyTaskId.NewTaskId(),
            Description = description,
            CreatedAt = DateTime.UtcNow,
            State = TaskState.Pending
        };

        return task;
    }

    public enum TaskState
    {
        Pending, Started, Done
    }
}
