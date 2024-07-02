namespace TaskTitan.Infrastructure.Dtos;

public class TaskItemDto
{
    public string Id { get; private set; } = string.Empty;
    public int RowId { get; set; }
    public string Description { get; set; } = string.Empty;
    public TaskItemState State { get; set; }
    // TODO: Add support for projects
    //  public string? Project { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public DateTime? Due { get; set; }
    public DateTime? Until { get; set; }
    public DateTime? Wait { get; set; }
    public DateTime? End { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? Scheduled { get; set; }

    public static TaskItemDto FromTaskItem(TaskItem task)
    {
        return new TaskItemDto
        {
            Id = task.Id.ToString(),
            RowId = task.RowId,
            Description = task.Description,
            Created = task.Entry,
            State = task.Status,
            Due = task.Due ?? null,
        };
    }
}

public class TaskItemModifyDto
{
    public string? Description { get; set; }
    public TaskItemState? State { get; set; }
    public string? DueText { get; set; }
    public string? UntilText { get; set; }
    public string? WaitText { get; set; }
    public string? EndText { get; set; }
    public string? StartText { get; set; }
    public string? ScheduledText { get; set; }
}
