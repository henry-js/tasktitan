using TaskTitan.Core;

namespace TaskTitan.Lib;

public interface ITaskRepository
{
    public void Add(MyTask task);
    public void Update(MyTask task);
    public IEnumerable<MyTask> Get();
    public MyTask Get(string id);
    public MyTask Delete(MyTask task);
}
