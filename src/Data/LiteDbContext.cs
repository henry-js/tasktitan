using System.Collections;
using System.Linq;

using LiteDB;

using TaskTitan.Configuration;
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Extensions;

namespace TaskTitan.Data;

public class LiteDbContext
{
    public const string FILE_NAME = "tasktitan.db";
    private const string TaskCol = "tasks";
    private readonly LiteDatabase db;
    private const string WorkingSetQuery = "SELECT $ FROM tasks WHERE Status = \"Pending\";";
    public LiteDbContext(string connectionString)
    {
        connectionString = $@"Filename={Global.DataDirectoryPath}\tasktitan.db;Connection=shared";
        try
        {
            var db = new LiteDatabase(connectionString);
            this.db = db ?? throw new Exception(nameof(db));
        }
        catch (Exception ex)
        {
            throw new Exception("Can't find or create LiteDb database.", ex);
        }

        var tasks = db.GetCollection<TaskItem>(TaskCol, BsonAutoId.ObjectId);

        tasks.EnsureIndex(x => x.Id, false);
        tasks.EnsureIndex(x => x.Status, false);
        // tasks.EnsureIndex(x => x.Recur, false);

    }

    public ILiteCollection<TaskItem> Tasks => db.GetCollection<TaskItem>(TaskCol, BsonAutoId.ObjectId);

    public IEnumerable<TaskItem> WorkingSet
    {
        get
        {
            var tasks = Tasks.Find(Query.Or(
                Query.EQ("Status", TaskItemStatus.Pending.ToString())))
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

    public int AddTask(IEnumerable<TaskAttribute> properties)
    {
        var id = ObjectId.NewObjectId();
        var task = new BsonDocument();
        task["_id"] = id;
        task[nameof(TaskItem.Entry)] = id.CreationTime;

        foreach (var propp in properties)
        {
            switch (propp.Name)
            {
                case nameof(TaskItem.Entry):
                case nameof(TaskItem.TaskId):
                    continue;
            }
            if (propp is TaskAttribute<DateTime> dateProp)
            {
                task[propp.Name] = dateProp.Value;
            }
            else if (propp is TaskAttribute<string> stringProp)
            {
                task[propp.Name] = stringProp.Value;
            }
            else if (propp is TaskAttribute<double> numProp)
            {
                task[propp.Name] = numProp.Value;
            }
        }
        if (!task.ContainsKey(nameof(TaskItem.Status))) task[nameof(TaskItem.Status)] = TaskItemStatus.Pending.ToString();
        task[nameof(TaskItem.Tags)] = new BsonArray(properties.Where(p => p is TaskTag).Select(t => new BsonValue(t.Name)));

        var tasks = db.GetCollection(TaskCol);
        int workingSetCount = tasks.Count(Query.EQ("Status", TaskItemStatus.Pending.ToString()));
        task["Id"] = workingSetCount + 1;
        tasks.Insert(task);
        return task["Id"];
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

    public IEnumerable<TaskItem> QueryTasks(FilterExpression query)
    {
        var tasks = Tasks.Find(query.ToBsonExpression());

        return tasks;
        // foreach (var (task, index) in tasks.Index())
        // {

        // }
    }
}
