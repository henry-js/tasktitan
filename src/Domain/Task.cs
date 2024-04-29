using shortid;

namespace TaskTitan.Domain;

public class Task
{
    private Task() { }
    public Task(string description)
    {
        Description = description;
    }

    public TaskId Id { get; private set; } = TaskId.Empty;
    public string Description { get; private set; }


    public Task CreateNew(string description)
    {
        Task task = new Task()
        {
            Id = TaskId.NewTaskId(),
            Description = description,
        };

        return task;
    }
}
