namespace TaskTitan.Lib.Services;

public interface ITaskItemService
{
    Task<int> Add(TaskItem task);
    Task<TaskItem?> Get(int rowId);
    // Task<IEnumerable<TaskItem>> GetTasks();
    Task<TaskItemResult> Update(TaskItem task);
    Task Delete(int rowId);
    Task Delete(TaskItem taskToDelete);
    Task<TaskItem?> Find(TaskItemId id);
    Task<IEnumerable<TaskItem>> GetTasks(IEnumerable<string> filters);
}
