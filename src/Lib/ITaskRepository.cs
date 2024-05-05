using TaskTitan.Core;

namespace TaskTitan.Lib;

public interface ITaskRepository
{
    public void Add(Task task);
    public void Update(Task task);
    public IEnumerable<Task> Get();
    public Task Get(string id);
    public Task Delete(Task task);
}
