using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TaskTitan.Core;

public class TaskItem
{
    protected TaskItem() { }

    public TaskItemId Id { get; private set; } = TaskItemId.Empty;

    [NotMapped]
    public int RowId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public TaskItemState State { get; private set; }
    public DateTime Created { get; private set; }
    public DateTime? Modified { get; private set; }
    public DateTime? Due { get; set; }
    public DateTime? Until { get; set; }
    public DateTime? Wait { get; set; }
    public DateTime? Started { get; set; }
    public DateTime? Ended { get; set; }
    public DateTime? Scheduled { get; set; }
    public TaskItemMetadata? Metadata { get; set; }

    public static TaskItem CreateNew(string description, TaskItemMetadata? metadata = null)
    {
        TaskItem task = new()
        {
            Id = TaskItemId.NewTaskId(),
            Description = description,
            // Created = DateTime.UtcNow,
            State = TaskItemState.Pending,
        };

        return task;
    }

    public TaskItem Start()
    {
        this.Started = DateTime.UtcNow;
        return this;
    }

    public TaskItem Complete()
    {
        State = TaskItemState.Completed;
        return this;
    }

    // public static TaskItem FromPending(TaskItem pendingTask)
    // {
    //     return new()
    //     {
    //         Id = pendingTask.Id,
    //         Description = pendingTask.Description,
    //         Created = pendingTask.Created,
    //         State = pendingTask.State,
    //         Due = pendingTask.Due,
    //     };
    // }

    public TaskItem WithIndex(int index)
    {
        this.RowId = index;
        return this;
    }

    public override string ToString()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
        return json;
    }
}
