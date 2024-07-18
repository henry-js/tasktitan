namespace TaskTitan.Core.Enums;

public record struct TaskItemAttribute(string Value)
{
    public static TaskItemAttribute Id => TaskItemConstants.Field.id;
    public static TaskItemAttribute Description => TaskItemConstants.Field.description;
    public static TaskItemAttribute Status => TaskItemConstants.Field.status;
    public static TaskItemAttribute Project => TaskItemConstants.Field.project;
    public static TaskItemAttribute Due => TaskItemConstants.Field.due;
    // public static TaskItemAttribute Recur => TaskItemConstants.Field.Recur;
    public static TaskItemAttribute Until => TaskItemConstants.Field.until;
    public static TaskItemAttribute Limit => TaskItemConstants.Field.limit;
    public static TaskItemAttribute Wait => TaskItemConstants.Field.wait;
    public static TaskItemAttribute Entry => TaskItemConstants.Field.entry;
    public static TaskItemAttribute End => TaskItemConstants.Field.end;
    public static TaskItemAttribute Start => TaskItemConstants.Field.start;
    public static TaskItemAttribute Scheduled => TaskItemConstants.Field.scheduled;
    public static TaskItemAttribute Modified => TaskItemConstants.Field.modified;
    public static TaskItemAttribute Depends => TaskItemConstants.Field.depends;
    public static TaskItemAttribute Tag => TaskItemConstants.Field.tag;
    public static TaskItemAttribute Empty => TaskItemConstants.Field.empty;

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

public static class TaskItemConstants
{
    public static class Field
    {
        public const string id = "id";
        public const string description = "description";
        public const string status = "status";
        public const string project = "project";
        public const string due = "due";
        // public const string recur = "Recur";
        public const string until = "until";
        public const string limit = "limit";
        public const string wait = "wait";
        public const string entry = "entry";
        public const string end = "end";
        public const string start = "start";
        public const string scheduled = "scheduled";
        public const string modified = "modified";
        public const string depends = "depends";
        public const string tag = "tag";
        public const string empty = "empty";
    }
}
