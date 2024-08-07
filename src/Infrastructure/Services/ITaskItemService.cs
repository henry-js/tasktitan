using TaskTitan.Core.OperationResults;

namespace TaskTitan.Infrastructure.Services;

public interface ITaskItemService
{
    Task<int> Add(ITaskRequest request);
    Task<Result> Delete(TaskItem taskToDelete);
    Task<Result<int>> Delete(ITaskRequest request);
    Task<IEnumerable<TaskItem>> GetTasks(IEnumerable<string> filters);
    Task<int> Update(ITaskRequest request);
}
public enum Action
{ Create, Update, Delete }
