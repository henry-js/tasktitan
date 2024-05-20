using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;

namespace TaskTitan.Core;

public class TTask
{
    protected TTask() { }
    public TTask(string description)
    {
        Description = description;
    }

    public TTaskId Id { get; private set; } = TTaskId.Empty;

    [NotMapped]
    public int RowId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public TTaskState State { get; private set; }
    public DateOnly? DueDate { get; set; }
    public TTaskMetadata? Metadata { get; set; }

    public static TTask CreateNew(string description, TTaskMetadata? metadata = null)
    {
        TTask task = new()
        {
            Id = TTaskId.NewTaskId(),
            Description = description,
            CreatedAt = DateTime.UtcNow,
            State = TTaskState.Pending,
            Metadata = metadata,
        };

        return task;
    }

    public TTask Start()
    {
        State = TTaskState.Started;
        return this;
    }

    public TTask Complete()
    {
        State = TTaskState.Done;
        return this;
    }

    public static TTask FromPending(TTask pendingTask)
    {
        return new()
        {
            Id = pendingTask.Id,
            Description = pendingTask.Description,
            CreatedAt = pendingTask.CreatedAt,
            State = pendingTask.State,
            DueDate = pendingTask.DueDate,
        };
    }

    public TTask WithIndex(int index)
    {
        this.RowId = index;
        return this;
    }
}
