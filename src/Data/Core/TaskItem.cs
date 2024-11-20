
using System.Reflection;

using LiteDB;

using TaskTitan.Data.Expressions;

namespace TaskTitan.Core;

public class TaskItem
{
    public TaskItem()
    {

    }
    private readonly Dictionary<string, object?> data = [];
    private bool updatedModified;

    public int Id { get; set; }
    [BsonId]
    public required ObjectId TaskId { get; set; }
    public required string Description { get; set; }
    public string? Depends { get; set; }
    public string? Priority { get; set; }
    public DateTime? Due { get; set; }
    public DateTime? End { get; set; }
    public DateTime Entry { get; init; }
    public DateTime? Modified { get => data["modified"] as DateTime?; set => data["modified"] = value; }
    public string? Project { get; set; }
    public Recurrence? Recur { get; set; }
    public DateTime? Scheduled { get; set; }
    public DateTime? Start { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public HashSet<string> Tags { get; set; } = [];
    public DateTime? Until { get; set; }
    public double Urgency { get; set; }
    public DateTime? Wait { get; set; }
    public Dictionary<string, TaskAttribute>? Udas { get; set; }

    public TaskItem ApplyBuiltIn(IEnumerable<TaskAttribute> attributes)
    {
        foreach (var attr in attributes.Where(a => a.AttributeKind == AttributeKind.BuiltIn))
        {
            var prop = GetType().GetProperty(attr.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            object value = attr switch
            {
                TaskAttribute<DateTime> dt => dt.Value,
                TaskAttribute<string> s => s.Value,
                TaskAttribute<double> d => d.Value,
                _ => throw new Exception("Unknown task attribute type " + attr.GetType())
            };
            prop?.SetValue(this, value);
        }
        foreach (var attr in attributes.Where(a => a.AttributeKind == AttributeKind.UserDefined))
        {
            Udas ??= [];
            Udas.TryAdd(attr.Name, attr);
        }
        foreach (var attr in attributes.Where(a => a.AttributeKind == AttributeKind.Tag))
        {
            if (attr is Tag tagAttr) Tags.Add(tagAttr.Value);
        }
        return this;
    }

    public void SetValue(string property, string value, Operations? ops = null)
    {
        // Update the modified timestamp unless setting it explicitly
        if (property != "modified" && !updatedModified)
        {
            string now = DateTime.UtcNow.ToString("o");
            // ops.Trace($"task: set property modified={now}");
            Update("modified", now, ops);
            updatedModified = true;
        }

        // Set or remove the property based on the presence of value
        if (value != null)
        {
            // ops.Trace($"task: set property {property}={value}");
            Update(property, value, ops);
        }
        else
        {
            // ops.Trace($"task: remove property {property}");
            data.Remove(property);
        }
    }
    private void Update(string property, string value, Operations? ops = null)
    {
        if (data.ContainsKey(property))
            data[property] = value;
        else
            data[property] = value;
    }
}

public class Operations;

public enum TaskItemStatus
{
    Pending, Completed, Deleted
}

public class Recurrence
{
}
