using System.Collections;
using System.Reflection.Metadata;
using LiteDB;
using TaskTitan.Configuration;

namespace TaskTitan.Data;

public class LiteDbContext
{
    public const string FILE_NAME = "tasktitan.db";
    private readonly LiteDatabase db;
    public LiteDbContext(string connectionString)
    {
        connectionString = $@"Filename={Global.DataDirectoryPath}\tasktitan.db";
        try
        {
            var db = new LiteDatabase(connectionString);
            if (db != null)
                this.db = db;
            else throw new ArgumentNullException(nameof(db));
        }
        catch (Exception ex)
        {
            throw new Exception("Can't find or create LiteDb database.", ex);
        }

        db.GetCollection<TaskItem>("tasks", BsonAutoId.Guid).EnsureIndex(x => x.Id, false);

    }

    public ILiteCollection<TaskItem> Tasks => db.GetCollection<TaskItem>("tasks", BsonAutoId.Guid);
    public IEnumerable<TaskItem> WorkingSet
    {
        get
        {
            var tasks = Tasks.Find(Query.Not("Status", TaskItemStatus.Pending.ToString()))
                .OrderBy(x => x.Entry)
                .Select((task, i) => { task.Id = i + 1; return task; });
            foreach (var task in tasks)
            {
                Tasks.Update(task);
            }
            return tasks;
        }
    }

    public static string CreateConnectionStringFrom(string dataDirectoryPath)
    {
        return $@"Filename={Path.Combine(dataDirectoryPath, FILE_NAME)}";
    }

    public IEnumerable<TaskItem> ListFromFilter(string linqFilterText, bool rebuildIds)
    {
        var tasks = Tasks.Find(linqFilterText);
        if (rebuildIds)
        {
            tasks = tasks.OrderBy(t => t.Entry)
                .Select((t, i) =>
                {
                    t.Id = i + 1;
                    return t;
                });
        }
        return tasks;
    }
}
