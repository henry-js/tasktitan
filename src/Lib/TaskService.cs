using TaskTitan.Core;
using TaskTitan.Data;

namespace TaskTitan.Lib.Services;

public class TaskService(TaskTitanDbContext dbcontext) : ITtaskService
{
    private readonly TaskTitanDbContext _dbcontext = dbcontext;

    public int Add(TTask task)
    {
        _dbcontext.Tasks.Add(task);
        _dbcontext.SaveChanges();

        return _dbcontext.PendingTasks.Count();
    }

    public TTask? Get(int rowId)
    {
        return _dbcontext.PendingTasks.FirstOrDefault(t => t.RowId == rowId);
    }

    public DateTime? Parse(string? dueDate) => dueDate switch
    {
        null => null,
        "eom" => new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)),
        var dueString when StringDateParser.IsDayOfMonth(dueString) => StringDateParser.ToDayOfMonth(dueString),
        _ => null,

    };


    public TTaskResult Update(int rowId, string? dueDate)
    {
        var task = Get(rowId);
        if (task == null) return new TTaskResult(false, task, "Could not find task");
        var due = Parse(dueDate);
        if (due == null) return new TTaskResult(false, task, "Could not parse due date option");
        task.DueDate = DateOnly.FromDateTime(due.Value);
        _dbcontext.Update(task);
        _dbcontext.Commit();
        return new TTaskResult(task != null, task);
    }
}
