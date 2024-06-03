using System.Diagnostics;

namespace TaskTitan.Cli.TaskCommands.Models;

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
    public IList<TaskItemDto>? Depends { get; set; }

    public static TaskItemDto FromTaskItem(Core.TaskItem task)
    {
        return new TaskItemDto
        {
            Id = task.Id.ToString(),
            RowId = task.RowId,
            Description = task.Description,
            Created = task.Created,
            State = task.State,
            Due = task.Due ?? null,
        };
    }
}
