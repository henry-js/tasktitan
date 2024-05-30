using TaskTitan.Lib.Text;

namespace TaskTitan.Lib.Services;

public class TaskItemService(ITaskItemRepository repository, TaskTitanDbContext dbContext, ILogger<TaskItemService> logger) : ITaskItemService
{
    private readonly ITaskItemRepository _repository = repository;
    private readonly TaskTitanDbContext dbContext = dbContext;
    private readonly ILogger<TaskItemService> _logger = logger;

    public int Add(TaskItem task)
    {
        try
        {
            var result = _repository.AddAsync(task).Result;
            if (result.IsSuccess) return 1;
            else return 0;
        }
        catch (System.Exception ex)
        {
            _logger.LogError("Save failed: {exception}", ex.Message);
            return -1;
        }
    }

    public void Delete(int rowId)
    {
        _logger.LogInformation("deleting Task {rowid}", rowId);
        var task = GetTasks().SingleOrDefault(t => t.RowId == rowId);

        if (task is null)
        {
            _logger.LogWarning("Task not found");
            return;
        }

        dbContext.Tasks.Remove(task);
        _logger.LogInformation("Task deleted");
    }

    public void Delete(TaskItem taskToDelete)
    {
        dbContext.Tasks.Remove(taskToDelete);
        dbContext.Commit();
    }

    public TaskItem? Find(TaskItemId id)
    {
        return dbContext.Tasks.SingleOrDefault(t => t.Id == id);
    }

    public TaskItem? Get(int rowId)
    {
        var task = GetTasks()
        .FirstOrDefault(t => t.RowId == rowId);
        if (task == null)
        {
            _logger.LogInformation("Task {rowId} not found", rowId);
        }
        return task;
    }

    public IEnumerable<TaskItem> GetTasks()
    {
        return _repository.GetAllAsync().Result;
    }

    public IEnumerable<TaskItem> GetTasks(List<ITaskQueryFilter> filters)
    {
        IEnumerable<TaskItem> queryable = GetTasks();
        foreach (var filter in filters)
        {
            if (filter is IdQueryFilter idFilter)
            {
                queryable = queryable.Where(t => idFilter.SoleIds.Contains(t.RowId));
            }
        }
        return queryable;
    }

    public TaskItemResult Update(TaskItem pendingTask)
    {
        List<string> errors = [];

        _logger.LogInformation("Updating task {rowId}", pendingTask.RowId);
        try
        {
            dbContext.Tasks.Update(pendingTask);
            dbContext.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return TaskItemResult.Fail(ex.Message);
        }
        return TaskItemResult.Success();
    }
}
