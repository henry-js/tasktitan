using System.Collections;
using System.Reflection.Metadata;

using LiteDB;

using TaskTitan.Configuration;
using TaskTitan.Data.Expressions;

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

    public int AddTask(IEnumerable<TaskProperty> properties)
    {
        var taskDoc = new BsonDocument();
        taskDoc["_id"] = Guid.NewGuid();
        foreach (var propp in properties)
        {
            if (propp is TaskProperty<DateTime> dateProp)
            {
                taskDoc[propp.PropertyName] = dateProp.Value;
            }
            else if (propp is TaskProperty<string> stringProp)
            {
                taskDoc[propp.PropertyName] = stringProp.Value;
            }
            else if (propp is TaskProperty<double> numProp)
            {
                taskDoc[propp.PropertyName] = numProp.Value;
            }
        }
        taskDoc[nameof(TaskItem.Tags)] = new BsonArray(properties.Where(p => p is TaskTag).Select(t => new BsonValue(t.Name)));
        taskDoc[nameof(TaskItem.Id)] = WorkingSet.Count();
        db.GetCollection("tasks", BsonAutoId.Guid).Insert(taskDoc);


        return WorkingSet.Count();
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
