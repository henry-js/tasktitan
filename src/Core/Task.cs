namespace TaskTitan.Core;

public class TTask
{
    private TTask() { }
    public TTask(string description)
    {
        Description = description;
    }

    public TTaskId Id { get; private set; } = TTaskId.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public TTaskState State { get; private set; }

    public static TTask CreateNew(string description)
    {
        TTask task = new()
        {
            Id = TTaskId.NewTaskId(),
            Description = description,
            CreatedAt = DateTime.UtcNow,
            State = TTaskState.Pending
        };

        return task;
    }
}

public enum TTaskState
{
    Pending, Started, Done
}
