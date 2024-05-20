namespace TaskTitan.Lib.Services;

public class TaskService(TaskTitanDbContext dbcontext, ILogger<TaskService> logger) : ITtaskService
{
    private readonly TaskTitanDbContext _dbcontext = dbcontext;
    private readonly ILogger<TaskService> _logger = logger;

    public int Add(TTask task)
    {
        _dbcontext.Tasks.Add(task);
        _dbcontext.SaveChanges();

        return _dbcontext.Tasks.AsNoTracking().Count();
    }

    public void Delete(int rowId)
    {
        _logger.LogInformation("deleting Task {rowid}", rowId);
        var tasks = GetTasks();
        var task = tasks.SingleOrDefault(t => t.RowId == rowId);

        if (task is null)
        {
            _logger.LogInformation("Task not found");
            return;
        }

        _dbcontext.Tasks.Remove(task);
        _logger.LogInformation("Task deleted");
    }

    public TTask? Get(int rowId, bool asreadonly = true)
    {
        var task = GetTasks(asreadonly)
        .FirstOrDefault(t => t.RowId == rowId);
        if (task == null)
        {
            _logger.LogInformation("Task {rowId} not found", rowId);
        }
        return task;
    }

    public IEnumerable<TTask> GetTasks(bool asreadonly = true)
    {
        int index = 1;
        return _dbcontext.Tasks.AsNoTracking().AsEnumerable().Select(t => t.WithIndex(index++));
    }

    public TTaskResult Update(TTask pendingTask)
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
            return TTaskResult.Fail(ex.Message);
        }
        return TTaskResult.Success();
    }
}
