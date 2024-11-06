using LiteDB;

namespace TaskTitan.Core;

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
    public Dictionary<string, UdaValue>? UserDefinedAttributes { get; set; }
}

public class UdaValue
{
    public string? Text { get; set; }
    public double? Number { get; set; }
    public DateTime? Date { get; set; }
    public UdaValue(string text) => Text = text;
    public UdaValue(double number) => Number = number;
    public UdaValue(DateTime date) => Date = date;

    // Allows implicit conversions for easy assignment
    public static implicit operator UdaValue(string text) => new(text);
    public static implicit operator UdaValue(double number) => new(number);
    public static implicit operator UdaValue(DateTime date) => new(date);
}

public enum TaskItemStatus { Pending, Started, Done }

public class Recurrence
{
}
