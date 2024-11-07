using LiteDB;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TaskTitan.Core;
using TaskTitan.Core.Enums;
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Extensions;

namespace TaskTitan.Data;

public class LiteDbContext
{
    private const StringComparison _ignoreCase = StringComparison.InvariantCultureIgnoreCase;
    private const string TaskCol = "tasks";
    private readonly LiteDatabase db;
    private readonly ILogger<LiteDbContext> logger;
    private readonly LiteDbOptions _options;
    public LiteDbContext(IOptions<LiteDbOptions> options, ILogger<LiteDbContext> logger)
    {
        BsonMapper.Global.RegisterType<Dictionary<string, UdaValue>>
            (serialize: LiteDbMappers.ToBsonDocument,
            deserialize: LiteDbMappers.FromBsonDocument);
        this.logger = logger;
        _options = options.Value;
        try
        {
            var db = new LiteDatabase(_options.ConnectionString);
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

    public int AddTask(IEnumerable<TaskAttribute> properties)
    {
        var id = ObjectId.NewObjectId();
        var task = new BsonDocument
        {
            ["_id"] = id,
            [TaskColumns.Entry] = id.CreationTime
        };
        var tags = properties.Where(p => p.AttributeKind == AttributeKind.Tag && p.Modifier == ColModifier.Include)
            .Select(t => new BsonValue(t.Name))
            .ToHashSet();

        task[TaskColumns.Tags] = new BsonArray(tags);

        foreach (var propp in properties.Where(p => p.Name != TaskColumns.Entry && p.Name != TaskColumns.TaskId))
        {
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
        if (!task.ContainsKey(TaskColumns.Status)) task[TaskColumns.Status] = TaskItemStatus.Pending.ToString();
        // task["UserDefinedAttributes"] = BsonMapper.Global.ToDocument(typeof(Dictionary<string, UdaValue>), )

        task["UserDefinedAttributes"] = properties.Where(p => p.AttributeKind == AttributeKind.UserDefined)
            .ToDictionary(t => t.Name);

        var tasks = db.GetCollection(TaskCol, BsonAutoId.ObjectId);
        int workingSetCount = tasks.Count(Query.EQ("Status", TaskItemStatus.Pending.ToString()));
        task["Id"] = workingSetCount + 1;
        tasks.Insert(task);
        return task["Id"];
    }

    // private void GetTagValue(BsonDocument task, TaskAttribute propp)
    // {
    //     if (propp is not TaskTag tag) throw new InvalidCastException($"Cannot cast {propp.GetType()} to {typeof(TaskTag)}");

    //     if (tag.Modifier == Core.Enums.ColModifier.Include)
    //     {
    //         if (task.TryGetValue(nameof))
    //     }
    // }

    // public IEnumerable<TaskItem> ListFromFilter(string linqFilterText, bool rebuildIds)
    // {
    //     var tasks = Tasks.Find(linqFilterText);
    //     if (rebuildIds)
    //     {
    //         tasks = tasks.OrderBy(t => t.Entry)
    //             .Select((t, i) =>
    //             {
    //                 t.Id = i + 1;
    //                 return t;
    //             });
    //     }
    //     return tasks;
    // }

    public IEnumerable<TaskItem> QueryTasks(FilterExpression query)
    {
        var bson = query.ToBsonExpression();
        logger.LogInformation("Generated query: {query}", bson);
        var untyped = db.GetCollection("tasks", BsonAutoId.ObjectId).Find(bson).ToList();

        var tasks = Tasks.Find(query.ToBsonExpression()).ToList();
        logger.LogInformation("Fetched rows {rows}", untyped.Count());
        return tasks;
        // foreach (var (task, index) in tasks.Index())
        // {

        // }
    }
}

public static class LiteDbMappers
{
    internal static Dictionary<string, UdaValue> FromBsonDocument(BsonValue value)
    {
        throw new NotImplementedException();
    }

    internal static BsonValue ToBsonDocument(Dictionary<string, UdaValue> dictionary)
    {
        var bsonDict = dictionary.ToDictionary(
                kvp => kvp.Key,
                kvp => new BsonValue(kvp.Value)
            );
        return new BsonDocument(bsonDict);
    }
}
