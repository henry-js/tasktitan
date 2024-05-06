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

}
