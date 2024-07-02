namespace TaskTitan.Infrastructure.Services;

public interface ITaskItemService
{
    Task<int> Add(ITaskRequest request);
    Task Delete(int rowId);
    Task Delete(TaskItem taskToDelete);
    Task<int> Delete(ITaskRequest request);
    Task<IEnumerable<TaskItem>> GetTasks(IEnumerable<string> filters);
    Task<int> Update(ITaskRequest request);
}
public enum Action
{ Create, Update, Delete }
