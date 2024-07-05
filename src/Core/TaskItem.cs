using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TaskTitan.Core;

public class TaskItem : INotifyPropertyChanged
{
    private string _description = string.Empty;

    protected TaskItem() { }

    public TaskItemId Id { get; private set; } = TaskItemId.Empty;

    [NotMapped]
    public int RowId { get; private set; }
    public string Description { get => _description; private set => _description = value; }
    public string? Project { get; set; }
    public TaskItemState Status { get; private set; }
    public TaskDate Entry { get; private set; }
    public TaskDate? Modified { get; private set; }
    public TaskDate? Due { get; set; }
    public TaskDate? Until { get; set; }
    public TaskDate? Wait { get; set; }
    public TaskDate? Start { get; set; }
    public TaskDate? End { get; set; }
    public TaskDate? Scheduled { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

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
