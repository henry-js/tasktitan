
using System.Collections;

using TaskTitan.Lib.Text;

namespace TaskTitan.Lib.Services;

public interface ITaskItemService
{
    int Add(TaskItem task);
    TaskItem? Get(int rowId, bool asreadonly = true);
    IEnumerable<TaskItem> GetTasks(bool asreadonly = true);
    TaskItemResult Update(TaskItem task);
    void Delete(int rowId);
    void Delete(TaskItem taskToDelete);
    TaskItem? Find(TaskItemId id);
    IEnumerable<TaskItem> GetTasks(List<ITaskQueryFilter> filters);
}
