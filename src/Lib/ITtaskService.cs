namespace TaskTitan.Lib.Services;

public interface ITtaskService
{
    int Add(TTask task);
    TTask? Get(int rowId);
    TTaskResult Update(TTask task);
}
