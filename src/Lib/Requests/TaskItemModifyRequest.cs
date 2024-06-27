using TaskTitan.Core.Enums;

namespace TaskTitan.Lib.Services;

public record TaskItemModifyRequest : ITaskRequest
{
    public string[] Filters { get; init; } = [];
    public Dictionary<string, object?> Attributes { get; init; } = [];
    public Action Operation { get; } = Action.Update;
}
