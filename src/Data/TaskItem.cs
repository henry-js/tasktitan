using LiteDB;

namespace TaskTitan.Data;

public class TaskItem
{
    public int Id { get; set; }
    [BsonId]
    public required ObjectId TaskId { get; set; }
    public required string Description { get; set; }
    public string? Depends { get; set; }
    public string? Priority { get; set; }
    public DateTime? Due { get; set; }
    public DateTime? End { get; set; }
    public DateTime Entry { get; init; }
    public DateTime? Modified { get; set; }
    public string? Project { get; set; }
    public Recurrence? Recur { get; set; }
    public DateTime? Scheduled { get; set; }
    public DateTime? Start { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public string[] Tags { get; set; } = [];
    public DateTime? Until { get; set; }
    public double Urgency { get; set; }
    public DateTime? Wait { get; set; }
}

public enum TaskItemStatus { Pending, Started, Done }

public class Recurrence
{
}

public class UserConfiguration
{
    public Dictionary<string, AttributeColumnConfig> ColumnConfiguration { get; set; } = [];
}
