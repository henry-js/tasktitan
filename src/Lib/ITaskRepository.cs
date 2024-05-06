using TaskTitan.Core;

namespace TaskTitan.Lib;

public interface ITaskRepository
{
    public void Add(TTask task);
    public void Update(TTask task);
    public IEnumerable<TTask> Get();
    public TTask Get(string id);
    public TTask Delete(TTask task);
    IEnumerable<TTask> GetAll();
}
