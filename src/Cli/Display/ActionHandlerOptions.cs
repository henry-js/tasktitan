using TaskTitan.Core;

namespace TaskTitan.Cli.Display;

public abstract class ActionHandlerOptions
{
    public bool SkipAll { get; set; }
    public bool Interactive => !SkipAll && !ConfirmAll;
    public bool ConfirmAll { get; set; }
    public required string ActionName { get; init; }
}
public class DeleteActionHandlerOptions : ActionHandlerOptions
{
    public required Func<TaskItem, Task<bool>> Action { get; init; }
}

public class StartActionHandlerOptions : ActionHandlerOptions
{
    public required Func<TaskItem, Task<bool>> Action { get; init; }
    public required Func<IEnumerable<TaskItem>, Task<bool>> BulkAction { get; init; }
    public required IEnumerable<TaskAttribute> Attributes { get; init; }

}
