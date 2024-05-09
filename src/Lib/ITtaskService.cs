using TaskTitan.Core;

namespace TaskTitan.Lib.Services;

public interface ITtaskService
{
    int Add(TTask task);
    TTask? Get(int rowId);
    TTaskResult Update(int rowId, string? dueDate);
}

public record TTaskResult(bool Success, TTask? task, string? Messaqge = null)
{

}
