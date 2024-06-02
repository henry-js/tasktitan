using TaskTitan.Core.Queries;

namespace TaskTitan.Core;

public interface ITaskItemRepository
{
    Task<int> AddAsync(TaskItem task);
    Task<IEnumerable<TaskItem>> GetByQueryFilter(IEnumerable<ITaskQueryFilter> queryFilters);
    Task<TaskItem?> GetById(TaskItemId id);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<int> UpdateAsync(TaskItem task);
    Task<int> UpdateRangeAsync(IEnumerable<TaskItem> tasks);
    Task<int> DeleteAsync(TaskItem task);
    Task<int> DeleteRangeAsync(IEnumerable<TaskItem> tasks);

}
