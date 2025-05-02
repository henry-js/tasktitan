using System.Data.Common;

using playground.Core;
using playground.Infrastructure.Data;

namespace playground.Commands;

internal class TaskCommands
{
    private readonly ITaskItemRepository _repo;

    public TaskCommands(ITaskItemRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from">int id from</param>
    /// <param name="to">int id to</param>
    /// <returns></returns>
    public async Task List(int from, int to)
    {
        var tasks = (await _repo.GetAllAsync()).Where(x => (int)x.Id > from && (int)x.Id < to).ToList();

        foreach (var task in tasks)
        {
            AnsiConsole.WriteLine(task.ToString());
        }
    }
}
