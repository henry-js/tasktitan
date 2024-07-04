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
    public DateTime Entry { get; private set; }
    public DateTime? Modified { get; private set; }
    public DateTime? Due { get; set; }
    public DateTime? Until { get; set; }
    public DateTime? Wait { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public DateTime? Scheduled { get; set; }

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

public readonly record struct TaskDateTime
{
    private readonly DateTime Value { get; init; }
    private bool IsDateOnly => Value.TimeOfDay == TimeSpan.MinValue;

    public TaskDateTime(DateTime dateTime)
    {
        if (dateTime == DateTime.MinValue) throw new ArgumentOutOfRangeException(nameof(dateTime), dateTime, "dateTime cannot be MinValue");
        if (dateTime == DateTime.MaxValue) throw new ArgumentOutOfRangeException(nameof(dateTime), dateTime, "dateTime cannot be MaxValue");
        if (dateTime.Kind != DateTimeKind.Utc) throw new ArgumentException("dateTime.Kind should be Utc", nameof(dateTime));

        Value = dateTime;
    }

    public TaskDateTime(DateOnly dateOnly) : this(dateOnly.ToDateTime(TimeOnly.MinValue)) { }

    public string ToString(string? format = null) => format is null ? ToString() : Value.ToString(format);

    public override string ToString() => IsDateOnly
        ? Value.ToString("yyyy-MM-dd")
        : Value.ToString("yyyy-MM-dd HH:mm:ss");

    public static implicit operator TaskDateTime(DateTime dateTime) => new(dateTime);
    public static implicit operator TaskDateTime(DateOnly dateOnly) => new(dateOnly);
}
