namespace TaskTitan.Core.Enums;

public record struct TaskItemAttribute(string Value)
{
    public static TaskItemAttribute Id => "id";
    public static TaskItemAttribute Description => "description";
    public static TaskItemAttribute Status => "status";
    public static TaskItemAttribute Project => "project";
    public static TaskItemAttribute Due => "due";
    // public static TaskItemAttribute Recur => "Recur";
    public static TaskItemAttribute Until => "until";
    public static TaskItemAttribute Limit => "limit";
    public static TaskItemAttribute Wait => "wait";
    public static TaskItemAttribute Entry => "entry";
    public static TaskItemAttribute End => "end";
    public static TaskItemAttribute Start => "start";
    public static TaskItemAttribute Scheduled => "scheduled";
    public static TaskItemAttribute Modified => "modified";
    public static TaskItemAttribute Depends => "depends";
    public static TaskItemAttribute Tag => "tag";
    public static TaskItemAttribute Empty => "empty";

    public static implicit operator TaskItemAttribute(string value)
    {
        return new TaskItemAttribute(value);
    }

    public static implicit operator string(TaskItemAttribute taskItemAttribute)
    {
        return taskItemAttribute.ToString()!;
    }

    public override readonly string ToString() => Value;
}
