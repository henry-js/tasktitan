
using LiteDB;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TaskTitan.Core;
using TaskTitan.Core.Configuration;
using TaskTitan.Core.Enums;
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Extensions;
using TaskTitan.Data.Parsers;

namespace TaskTitan.Data;

public class LiteDbContext
{
    private const string TaskCol = "tasks";
    private readonly LiteDatabase db;
    private readonly ILogger<LiteDbContext> logger;
    private readonly LiteDbOptions _options;
    private readonly DateParser _dateParser;


    public LiteDbContext(IOptions<LiteDbOptions> options, IOptions<TaskTitanConfig> appConfig, TimeProvider timeProvider, ILogger<LiteDbContext> logger)
    {
        BsonMapper.Global.RegisterType
            (serialize: LiteDbMappers.ToBsonDocument,
            deserialize: LiteDbMappers.FromBsonDocument(appConfig.Value.Uda));
        this.logger = logger;
        _options = options.Value;
        _dateParser = new DateParser(timeProvider);
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
    }

    public ILiteCollection<TaskItem> Tasks => db.GetCollection<TaskItem>(TaskCol, BsonAutoId.ObjectId);

    public int AddTask(IEnumerable<TaskAttribute> values)
    {
        var id = ObjectId.NewObjectId();
        var task = new BsonDocument
        {
            ["_id"] = id,
            [TaskColumns.Entry] = id.CreationTime
        };

        var tags = values.Where(p => p.AttributeKind == AttributeKind.Tag && p.Modifier == ColModifier.Include)
            .Select(t => new BsonValue(t.Name))
            .ToHashSet();
        task[TaskColumns.Tags] = new BsonArray(tags);

        foreach (var item in values.Where(val => val.AttributeKind == AttributeKind.BuiltIn))
        {
            task[item.Name] = RetrieveValue(item);
        }
        var udas = BsonMapper.Global.ToDocument(values.Where(val => val.AttributeKind == AttributeKind.UserDefined).ToDictionary(k => k.Name));

        task[nameof(udas)] = udas;
        if (!task.ContainsKey(TaskColumns.Status)) task[TaskColumns.Status] = TaskItemStatus.Pending.ToString();

        var tasks = db.GetCollection(TaskCol, BsonAutoId.ObjectId);
        int workingSetCount = tasks.Count(Query.EQ("Status", TaskItemStatus.Pending.ToString()));
        task["Id"] = workingSetCount + 1;
        tasks.Insert(task);
        return task["Id"];
    }

    private BsonValue RetrieveValue(TaskAttribute item)
    {
        if (item is TaskAttribute<DateTime> dateProp)
        {
            return new BsonValue(dateProp.Value);
        }
        else if (item is TaskAttribute<string> stringProp)
        {
            return new BsonValue(stringProp.Value);
        }
        else if (item is TaskAttribute<double> numProp)
        {
            return new BsonValue(numProp.Value);
        }

        throw new Exception($"Could not parse value: {item.Name}");
    }

    public IEnumerable<TaskItem> QueryTasks(FilterExpression? query = null)
    {
        if (query is null)
        {
            return Tasks.FindAll().OrderBy(t => t.Id).ToList();
        }
        var bson = query?.ToBsonExpression(_dateParser);
        logger.LogInformation("Generated query: {query}", bson);

        var tasks = Tasks.Find(bson).ToList();
        logger.LogInformation("Fetched rows {rows}", tasks.Count);
        return tasks;
    }

    public async Task NukeTaskAsync(TaskItem task)
    {
        await Task.Run(() => Tasks.Delete(task.TaskId));
    }

    public async Task<bool> DeleteTask(TaskItem item)
    {
        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateTask(TaskItem item)
    {
        return await Task.Run(() => Tasks.Update(item));
    }

    public async Task<bool> BulkUpdateTask(IEnumerable<TaskItem> tasks)
    {
        throw new NotImplementedException();
    }
}

public static class LiteDbMappers
{
    private static ConfigDictionary<AttributeDefinition> _udas = [];

    internal static Func<BsonValue, Dictionary<string, TaskAttribute>> FromBsonDocument(ConfigDictionary<AttributeDefinition> udas)
    {
        _udas = udas;
        return FromBsonDocument;
    }

    internal static Dictionary<string, TaskAttribute> FromBsonDocument(BsonValue value)
    {
        var dict = new Dictionary<string, TaskAttribute>();
        if (value is not BsonDocument doc) throw new Exception();
        foreach (var item in doc)
        {
            if (_udas.ContainsKey(item.Key))
            {
                var uda = _udas[item.Key];
                dict[item.Key] = _udas[item.Key].Type switch
                {
                    ColType.Date => new TaskAttribute<DateTime>(uda.Name, DateTime.Parse(item.Value), AttributeKind.UserDefined),
                    ColType.Text => new TaskAttribute<string>(uda.Name, item.Value, AttributeKind.UserDefined),
                    ColType.Number => new TaskAttribute<double>(uda.Name, Convert.ToDouble((string)item.Value), AttributeKind.UserDefined),
                    _ => throw new ArgumentException($"Unsupported column type: {uda.Name}")
                };
            }
        }
        return dict;
    }

    internal static BsonValue ToBsonDocument(Dictionary<string, TaskAttribute> dictionary)
    {
        var bsonDict = new Dictionary<string, BsonValue>();
        foreach (var kvp in dictionary)
        {
            if (kvp.Value is TaskAttribute<DateTime> dateProp)
            {
                bsonDict.Add(kvp.Key, dateProp.Value);
            }
            else if (kvp.Value is TaskAttribute<string> stringProp)
            {
                bsonDict.Add(kvp.Key, stringProp.Value);
            }
            else if (kvp.Value is TaskAttribute<double> numProp)
            {
                bsonDict.Add(kvp.Key, numProp.Value);
            }
        }
        return new BsonDocument(bsonDict);
    }
}
