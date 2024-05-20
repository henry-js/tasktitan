namespace TaskTitan.Lib.Services;

public interface ITtaskService
{
    int Add(TTask task);
    TTask? Get(int rowId, bool asreadonly = true);
    IEnumerable<TTask> GetTasks(bool asreadonly = true);
    TTaskResult Update(TTask task);
    void Delete(int rowId);
}
