using FluentResults;

namespace TaskTitan.Infrastructure.Services;

public interface ITaskItemService
{
    Task<Result<int>> Add(ITaskRequest request);
    Task<Result> Delete(TaskItem taskToDelete);
    Task<Result<int>> Delete(ITaskRequest request);
    Task<Result<IEnumerable<TaskItem>>> GetTasks(IEnumerable<string> filters);
    Task<Result<IEnumerable<TaskItem>>> GetTasks(ITaskRequest request);
    Task<Result<int>> Update(ITaskRequest request);
}
public enum Action
{
    Create, Update, Delete,
    Fetch
}
