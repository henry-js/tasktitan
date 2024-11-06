using LiteDB;

using TaskTitan.Core;
using TaskTitan.Data;

namespace TaskTitan.Benchmarks;

public class LiteDbTaskStore
{
    private readonly LiteDatabase _database;

    public LiteDbTaskStore(string databasePath)
    {
        _database = new LiteDatabase("Filename=:temp:");
    }

    public void InsertTask(TaskItem task)
    {
        var tasks = _database.GetCollection<TaskItem>("tasks");
        tasks.Insert(task);
    }

    public TaskItem GetTask(int id)
    {
        var tasks = _database.GetCollection<TaskItem>("tasks");
        return tasks.FindById(id);
    }
}
