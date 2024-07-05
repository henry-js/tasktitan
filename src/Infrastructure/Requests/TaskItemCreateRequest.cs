using TaskTitan.Core.Enums;

namespace TaskTitan.Infrastructure.Services;

public record TaskItemCreateRequest : ITaskRequest
{
    public string[] Filters { get; init; } = [];
    public Action Operation { get; } = Action.Create;
    // public Dictionary<TaskItemAttribute, string> Attributes { get; } = [];

    public required TaskItem Task { get; set; }
}

public record CreateTaskItemDto
{
    public required string Description { get; init; }
    public TaskItemState State { get; init; }
    public string? Due { get; init; }
    public string? Until { get; init; }
    public string? Wait { get; init; }
    public string? Started { get; init; }
    public string? Ended { get; init; }
    public string? Scheduled { get; init; }

}
