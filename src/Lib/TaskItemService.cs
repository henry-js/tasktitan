using TaskTitan.Lib.Text;

namespace TaskTitan.Lib.Services;

public class TaskItemService(TaskTitanDbContext dbcontext, ILogger<TaskItemService> logger) : ITaskItemService
{
    private readonly TaskTitanDbContext _dbcontext = dbcontext;
    private readonly ILogger<TaskItemService> _logger = logger;

    public int Add(TaskItem task)
    {
        try
        {
            _dbcontext.Tasks.Add(task);
            return _dbcontext.SaveChanges();
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
        var task = GetTasks(false).SingleOrDefault(t => t.RowId == rowId);

        if (task is null)
        {
            _logger.LogWarning("Task not found");
            return;
        }

        _dbcontext.Tasks.Remove(task);
        _logger.LogInformation("Task deleted");
    }

    public void Delete(TaskItem taskToDelete)
    {
        _dbcontext.Tasks.Remove(taskToDelete);
        _dbcontext.Commit();
    }

    public TaskItem? Find(TaskItemId id)
    {
        return _dbcontext.Tasks.SingleOrDefault(t => t.Id == id);
    }

    public TaskItem? Get(int rowId, bool asreadonly = true)
    {
        var task = GetTasks(asreadonly)
        .FirstOrDefault(t => t.RowId == rowId);
        if (task == null)
        {
            _logger.LogInformation("Task {rowId} not found", rowId);
        }
        return task;
    }

    public IEnumerable<TaskItem> GetTasks(bool asreadonly = true)
    {
        int index = 1;
        return _dbcontext.Tasks
            .ToList();
    }

    public IEnumerable<TaskItem> GetTasks(List<ITaskQueryFilter> filters)
    {
        IEnumerable<TaskItem> queryable = GetTasks(false);
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
            _dbcontext.Tasks.Update(pendingTask);
            _dbcontext.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return TaskItemResult.Fail(ex.Message);
        }
        return TaskItemResult.Success();
    }
}
