using LiteDB; // For BsonMapper registration later, or custom attributes if needed

using Vogen;

namespace TaskTitan.Core;

[ValueObject<int>]
public partial struct TaskId
{
}

[ValueObject<Guid>]
public partial struct TaskUuid { }

[ValueObject<string>]
public partial class ProjectName
{
    // Example Validation: Disallow empty/whitespace, maybe invalid chars
    private static Validation Validate(string value) =>
        string.IsNullOrWhiteSpace(value) ? Validation.Invalid("Project name cannot be empty.") : Validation.Ok;
}

[ValueObject<string>]
public readonly partial struct TagName
{
    private static Validation Validate(string value) =>
        value.Any(c => char.IsWhiteSpace(c) || c == ',') ? Validation.Invalid("Tag name cannot contain whitespace or commas.") : Validation.Ok;
}

[ValueObject<DateTimeOffset>] public readonly partial struct DueDate { }
[ValueObject<DateTimeOffset>] public readonly partial struct WaitDate { }
[ValueObject<DateTimeOffset>] public readonly partial struct ScheduledDate { }
[ValueObject<DateTimeOffset>] public readonly partial struct EntryDate { }
[ValueObject<DateTimeOffset>] public readonly partial struct ModifiedDate { }
[ValueObject<DateTimeOffset>] public readonly partial struct EndDate { }

[ValueObject<string>]
public readonly partial struct TaskDescription
{
    private static Validation Validate(string? value) =>
         string.IsNullOrEmpty(value) ? Validation.Invalid("Description cannot be empty.") : Validation.Ok;
}

public record TaskItem
(
    TaskUuid Uuid, // Use Guid directly or TaskUuid
    TaskId? Id,
    TaskDescription Description,
    ProjectName? Project,
    EntryDate Entry,
    ModifiedDate? Modified,
    EndDate? End,
    DueDate? Due,
    WaitDate? Wait,
    TaskItemStatus Status,
    List<TagName> Tags,
    ScheduledDate? Scheduled
)
{
    internal static TaskItem Create(TaskDescription taskDescription, ProjectName? projectName, TaskItemStatus pending, List<TagName> tagVos, DueDate? dueDateVo, EntryDate? entry = null,
    ModifiedDate? modified = null,
    EndDate? end = null,
    DueDate? due = null,
    WaitDate? wait = null)
    {
        throw new NotImplementedException();
    }
}

public enum TaskItemStatus
{
    Pending, Completed, Deleted
}