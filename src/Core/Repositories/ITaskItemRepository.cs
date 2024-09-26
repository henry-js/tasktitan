using TaskTitan.Core.Enums;

namespace TaskTitan.Core;

public interface ITaskItemRepository
{
    Task<int> AddAsync(TaskItem task);
    Task<IEnumerable<TaskItem>> GetByFilterAsync(IEnumerable<Expression> expressions);
    Task<int> UpdateByFilter(IEnumerable<Expression> expressions, IEnumerable<KeyValuePair<string, object?>> keyValues);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<int> DeleteAsync(TaskItem task);
    Task<int> DeleteByFilter(IEnumerable<Expression> filterExpressions);
    Task<IEnumerable<TaskItem>> GetTasks(IEnumerable<Expression> expressions, IEnumerable<TaskItemAttribute>? fields = null);
}
