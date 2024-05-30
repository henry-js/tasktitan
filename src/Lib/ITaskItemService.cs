
using System.Collections;

using TaskTitan.Lib.Text;

namespace TaskTitan.Lib.Services;

public interface ITaskItemService
{
    int Add(TaskItem task);
    TaskItem? Get(int rowId);
    IEnumerable<TaskItem> GetTasks();
    TaskItemResult Update(TaskItem task);
    void Delete(int rowId);
    void Delete(TaskItem taskToDelete);
    TaskItem? Find(TaskItemId id);
    IEnumerable<TaskItem> GetTasks(List<ITaskQueryFilter> filters);
}
