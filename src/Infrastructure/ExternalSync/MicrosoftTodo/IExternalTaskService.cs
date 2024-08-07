using Microsoft.Graph.Models;

using TaskTitan.Core.OperationResults;
using TaskTitan.Infrastructure.Dtos;

namespace TaskTitan.Infrastructure.ExternalSync.MicrosoftTodo;

public interface IExternalTaskService
{
    Task<Result<string>> CreateNewListAsync();
    Task<Result> ExportTaskAsync(string listId, TaskItemDto task);
    Task<Result<IEnumerable<TodoTask>>> FetchExistingExportedAsync(string listId);
    Task<IEnumerable<TodoTaskList>> GetAsync();
    Task<string?> GetListIdAsync();
    Task<IEnumerable<TodoTaskList>> GetListsAsync();
    Task<List<TaskItemDto>> GetTasksToExportAsync(bool all);
}
