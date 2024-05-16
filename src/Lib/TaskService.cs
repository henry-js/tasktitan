using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using TaskTitan.Core;
using TaskTitan.Data;

namespace TaskTitan.Lib.Services;

public class TaskService(TaskTitanDbContext dbcontext, DueDateHelper dateHelper, ILogger<TaskService> logger) : ITtaskService
{
    private readonly TaskTitanDbContext _dbcontext = dbcontext;
    private readonly DueDateHelper _dateHelper = dateHelper;
    private readonly ILogger<TaskService> _logger = logger;

    public int Add(TTask task)
    {
        _dbcontext.Tasks.Add(task);
        _dbcontext.SaveChanges();

        return _dbcontext.PendingTasks.AsNoTracking().Count();
    }

    public PendingTTask? Get(int rowId)
    {
        var task = _dbcontext.PendingTasks.AsNoTracking().FirstOrDefault(t => t.RowId == rowId);
        if (task == null)
        {
            _logger.LogInformation("Task not found"); _logger.LogInformation("Task {rowId} not found", rowId);
        }
        return task;
    }

    public TTaskResult UpdateDueDate(PendingTTask pendingTask, string dueString)
    {
        List<string> errors = [];
        if (dueString != string.Empty)
        {
            var due = _dateHelper.DateStringToDate(dueString);
            if (due is null)
            {
                _logger.LogWarning("Could not parse received dueDate: {dueDate}", dueString);
                errors.Add("Could not parse due date");
            }
            else
            {
                pendingTask.DueDate = due;
            }
        }
        else
        {
            _logger.LogInformation("Removing due date");
            pendingTask.DueDate = null;
        }
        var task = TTask.FromPending(pendingTask);
        _logger.LogInformation("Updating task {rowId}", pendingTask.RowId);
        _dbcontext.Tasks.Update(task);
        _dbcontext.Commit();
        return new TTaskResult(task != null, task);
    }
}
