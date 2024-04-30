namespace TaskTitan.Core;

public class Task
{
    private Task() { }
    public Task(string description)
    {
        Description = description;
    }

    public TaskId Id { get; private set; } = TaskId.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public static Task CreateNew(string description)
    {
        Task task = new()
        {
            Id = TaskId.NewTaskId(),
            Description = description,
            CreatedAt = DateTime.UtcNow,
        };

        return task;
    }
}
