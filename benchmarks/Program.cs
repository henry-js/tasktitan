using Bogus;
using Bogus.DataSets;
using TaskTitan.Configuration;
using TaskTitan.Data;
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Extensions;
using TaskTitan.Data.Parsers;
using TaskTitan.Data.Reports;
using Tomlyn;

var config = new ReportConfiguration();
var reports = config.Report;

File.WriteAllText(Path.Combine(Global.ConfigDirectoryPath, "reports.toml"), Toml.FromModel(reports));
reports = Toml.ToModel<ReportDictionary>(File.ReadAllText(Path.Combine(Global.ConfigDirectoryPath, "reports.toml")));

var context = new LiteDbContext(LiteDbContext.CreateConnectionStringFrom(Global.DataDirectoryPath));

var tasks = context.Tasks;

var sampleTasks = new Faker<TaskItem>()
    .CustomInstantiator(f => new TaskItem(f.Hacker.Phrase()))
    .RuleFor(t => t.Uuid, (f, u) => Guid.NewGuid())
    .RuleFor(t => t.Entry, (f, t) => f.Date.Between(new DateTime(2024, 01, 01), new DateTime(2024, 01, 08)))
    .RuleFor(t => t.Modified, (f, t) => f.Date.Recent(20))
    .RuleFor(t => t.Project, (f, t) => f.PickRandom(new List<string?> { "Work", "Home", "SideHustle", null }))
    .RuleFor(t => t.Due, (f, t) => f.Date.Future().OrNull(f, .2f))
    .RuleFor(t => t.Status, (f, t) => f.PickRandom<TaskItemStatus>())
    .RuleFor(t => t.Urgency, (f, t) => f.Random.Double() * 10)
    ;

var generatedTasks = sampleTasks.Generate(1000);

tasks.InsertBulk(generatedTasks);

var dateParser = new DateParser(TimeProvider.System);
var attribute1 = TaskProperty.Create("due.after", "tomorrow", dateParser);
var attribute2 = TaskProperty.Create("project.contains", "work", dateParser);
var expr = new FilterExpression(new BinaryFilter(attribute1, BinaryOperator.Or, attribute2));

var filtered = tasks.Find(expr.ToBsonExpression());

var workingSet = context.WorkingSet;

Console.WriteLine(filtered.Count());
