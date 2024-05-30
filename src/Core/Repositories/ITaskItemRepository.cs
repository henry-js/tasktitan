using TaskTitan.Core.OperationResults;

namespace TaskTitan.Core;

public interface ITaskItemRepository
{
    Task<Result> AddAsync(TaskItem task);
    Task<IEnumerable<TaskItem>> GetByQueryFilter(IEnumerable<ITaskQueryFilter> queryFilters);
    Task<TaskItem> GetById(TaskItemId id);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<Result> UpdateAsync(TaskItem task);
    Task<Result> UpdateRangeAsync(IEnumerable<TaskItem> tasks);
    Task<Result> DeleteAsync(TaskItem task);
    Task<Result> DeleteRangeAsync(IEnumerable<TaskItem> tasks);

}
