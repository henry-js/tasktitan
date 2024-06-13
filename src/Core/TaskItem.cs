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
    public string? Project { get; set; }
    public TaskItemState Status { get; private set; }
    public DateTime Entry { get; private set; }
    public DateTime? Modified { get; private set; }
    public DateTime? Due { get; set; }
    public DateTime? Until { get; set; }
    public DateTime? Wait { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public DateTime? Scheduled { get; set; }
    // [NotMapped]
    // public TaskItemMetadata? Metadata { get; set; }

    public static TaskItem CreateNew(string description, TaskItemMetadata? metadata = null)
    {
        TaskItem task = new()
        {
            Id = TaskItemId.NewTaskId(),
            Description = description,
            // Created = DateTime.UtcNow,
            Status = TaskItemState.Pending,
        };

        return task;
    }

    public TaskItem Begin()
    {
        this.Start = DateTime.UtcNow;
        return this;
    }

    public TaskItem Complete()
    {
        Status = TaskItemState.Completed;
        return this;
    }
}
