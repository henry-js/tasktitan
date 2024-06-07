using TaskTitan.Lib.Expressions;

namespace TaskTitan.Lib.Services;

public class TaskItemService(ITaskItemRepository repository, IExpressionParser parser, ILogger<TaskItemService> logger) : ITaskItemService
{
    private readonly ITaskItemRepository _repository = repository;
    private readonly IExpressionParser _parser = parser;
    private readonly ILogger<TaskItemService> _logger = logger;

    public async Task<int> Add(TaskItem task)
    {
        try
        {
            var result = await _repository.AddAsync(task);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("Save failed: {exception}", ex.Message);
            return -1;
        }
    }

    public async Task Delete(int rowId)
    {
        _logger.LogInformation("deleting Task {rowid}", rowId);
        var task = (await GetTasks([])).SingleOrDefault(t => t.RowId == rowId);

        if (task is null)
        {
            _logger.LogWarning("Task not found");
            return;
        }

        // dbContext.Tasks.Remove(task);
        _logger.LogInformation("Task deleted");
    }

    public async Task Delete(TaskItem taskToDelete)
    {
        await _repository.DeleteAsync(taskToDelete);
    }

    public async Task<TaskItem?> Find(TaskItemId id)
    {
        return await _repository.GetById(id);
    }

    public async Task<TaskItem?> Get(int rowId)
    {
        var task = (await GetTasks([]))
        .FirstOrDefault(t => t.RowId == rowId);
        if (task == null)
        {
            _logger.LogInformation("Task {rowId} not found", rowId);
        }
        return task;
    }

    public async Task<IEnumerable<TaskItem>> GetTasks(IEnumerable<string> filters)
    {
        if (!filters.Any())
        {
            return await _repository.GetByQueryFilter([]);
        }
        var filter = string.Join(" OR ", filters.Select(f => _parser.ParseFilter(f).ToQueryString()));
        return await _repository.GetByQueryFilter([filter]);
    }

    public async Task<TaskItemResult> Update(TaskItem pendingTask)
    {
        List<string> errors = [];

        _logger.LogInformation("Updating task {rowId}", pendingTask.RowId);
        try
        {
            await _repository.UpdateAsync(pendingTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return TaskItemResult.Fail(ex.Message);
        }
        return TaskItemResult.Success();
    }
}
