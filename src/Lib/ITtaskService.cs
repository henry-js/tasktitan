using TaskTitan.Core;

namespace TaskTitan.Lib.Services;

public interface ITtaskService
{
    int Add(TTask task);
    PendingTTask? Get(int rowId);
    TTaskResult UpdateDueDate(PendingTTask task, string dueDate);
}
