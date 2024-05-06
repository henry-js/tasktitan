using TaskTitan.Core;

namespace TaskTitan.Lib.Services;

public class TaskService(ITaskRepository repository) : ITtaskService
{
    private readonly ITaskRepository _repository = repository;

    public int AddTask(TTask task)
    {
        _repository.Add(task);
        return _repository.GetAll().Count();
    }

}
