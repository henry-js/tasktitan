using System.Collections;

using TaskTitan.Core.Enums;

namespace TaskTitan.Core;

public interface ITaskItemRepository
{
    Task<int> AddAsync(TaskItem task);
    // Task<IEnumerable<TaskItem>> GetByQueryFilter(string queryFilters);
    Task<IEnumerable<TaskItem>> GetByFilterAsync(IEnumerable<Expression> expressions);
    // Task<int> UpdateByFilter(Dictionary<TaskItemAttribute, string> values, string queryFilters);
    Task<int> UpdateByFilter(IEnumerable<Expression> expressions, IEnumerable<KeyValuePair<TaskItemAttribute, string>> keyValues);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<int> DeleteAsync(TaskItem task);
    Task<int> DeleteByFilter(IEnumerable<Expression> filterExpressions);
}
