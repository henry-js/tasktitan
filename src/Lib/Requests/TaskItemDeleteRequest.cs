namespace TaskTitan.Lib.Services;

public record TaskItemDeleteRequest : ITaskRequest
{
    public string[] Filters { get; init; } = [];
    public Action Operation { get; } = Action.Delete;
}
