using Bogus;

using LiteDB;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using TaskTitan.Core;
using TaskTitan.Core.Configuration;
using TaskTitan.Data;

var config = new TaskTitanConfig();
var reports = config.Report;
var ldbOptions = new LiteDbOptions();
Console.WriteLine(ldbOptions.ConnectionString);

var opts = Options.Create(ldbOptions);
var context = new LiteDbContext(opts, new NullLogger<LiteDbContext>());
var col = context.Tasks;
var tasks = context.Tasks.FindAll().ToList();
int rowId = tasks.Count != 0 ? tasks.Max(x => x.Id) : 0;
var sampleTasks = new Faker<TaskItem>()
    // .CustomInstantiator(f => new TaskItem(ObjectId.NewObjectId(), f.Hacker.Phrase()))
    .RuleFor(t => t.TaskId, (f, u) => ObjectId.NewObjectId())
    .RuleFor(t => t.Entry, (f, t) => f.Date.Between(new DateTime(2024, 01, 01), new DateTime(2024, 01, 08)))
    .RuleFor(t => t.Modified, (f, t) => f.Date.Recent(20))
    .RuleFor(t => t.Project, (f, t) => f.PickRandom(new List<string?> { "Work", "Home", "SideHustle", null }))
    .RuleFor(t => t.Due, (f, t) => f.Date.Future().OrNull(f, .2f))
    .RuleFor(t => t.Status, (f, t) => f.PickRandom<TaskItemStatus>())
    .RuleFor(t => t.Urgency, (f, t) => f.Random.Double() * 10)
    .RuleFor(t => t.Id, (f, t) => t.Status == TaskItemStatus.Pending ? ++rowId : 0)
    ;

var generatedTasks = sampleTasks.Generate(1000);

var count = col.InsertBulk(generatedTasks);

Console.WriteLine($"Inserted {count} tasks");

// var dateParser = new DateParser(TimeProvider.System);
// var attribute1 = TaskPropertyFactory.Create("due.after", "tomorrow", dateParser);
// var attribute2 = TaskPropertyFactory.Create("project.contains", "work", dateParser);
// var expr = new FilterExpression(new BinaryFilter(attribute1, BinaryOperator.Or, attribute2));

// var filtered = col.Find(expr.ToBsonExpression());

// // var workingSet = context.WorkingSet;

// Console.WriteLine(filtered.Count());
