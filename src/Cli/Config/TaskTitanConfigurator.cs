
using System.Collections;
using System.Diagnostics.CodeAnalysis;

using CsToml;
using CsToml.Formatter;

using TaskTitan.Core.Configuration;
using TaskTitan.Data.Reports;

using Velopack.Sources;

namespace TaskTitan.Cli.Config;

[TomlSerializedObject]
public partial class Configuration
{
    public Configuration()
    {
        ReportsA = (TaskTitanConfigurator.CreateDefaultReports() as ReportDictionary)!.Values.ToArray();
        // ReportsB = TaskTitanConfigurator.CreateDefaultReports(true) as ConfigDictionary<ReportDefinition>;
        Report1 = ReportsA.First();
        Report2 = ReportsA.Skip(1).Take(1).First();
        ReportDict = TaskTitanConfigurator.CreateDefaultReports() as ReportDictionary ?? [];
    }
    public static Configuration Default => new();

    // [TomlValueOnSerialized("table")]
    public ReportDefinition[] ReportsA { get; set; }
    [TomlValueOnSerialized]
    public ReportDefinition Report1 { get; set; }
    // [TomlValueOnSerialized]
    public ReportDefinition Report2 { get; set; }
    [TomlValueOnSerialized]
    public IDictionary<string, ReportDefinition> ReportDict { get; set; }
    // [TomlValueOnSerialized]
    // public IDictionary<string, ReportDefinition> ReportsB { get; set; }
}
public static class TaskTitanConfigurator
{
    public static IDictionary<string, ReportDefinition> CreateDefaultReports(bool forB = false)
    {
        if (!forB)
        {
            var reportDict = new ReportDictionary();

            reportDict.Add("report.active", new ReportDefinition()
            {
                Description = "Active tasks",
                Filter = "status:pending and +ACTIVE",
                Columns = ["id", "start", "start.age", "entry.age", "depends.indicator", "priority", "project", "tags", "recur", "wait", "scheduled.remaining", "due", "until", "description"],
                Labels = ["ID", "Started", "Active", "Age", "D", "P", "Project", "Tags", "Recur", "W", "Sch", "Due", "Until", "Description"],
            });

            reportDict.Add("report.all", new ReportDefinition()
            {
                Description = "All tasks",
                Filter = "",
                Columns = ["id", "status.short", "uuid.short", "start.active", "entry.age", "end.age", "depends.indicator", "priority", "project.parent", "tags.count", "recur.indicator", "wait.remaining", "scheduled.remaining", "due", "until.remaining", "description"],
                Labels = ["ID", "St", "UUID", "A", "Age", "Done", "D", "P", "Project", "Tags", "R", "Wait", "Sch", "Due", "Until", "Description"],
            });
            reportDict.Add("report.blocked", new ReportDefinition()
            {
                Description = "Blocked tasks",
                Filter = "status:pending -WAITING +BLOCKED",
                Columns = ["id", "depends", "project", "priority", "due", "start.active", "entry.age", "description"],
                Labels = ["ID", "Deps", "Proj", "Pri", "Due", "Active", "Age", "Description"],
            });
            reportDict.Add("report.blocking", new ReportDefinition()
            {
                Description = "Blocking tasks",
                Filter = "status:pending -WAITING +BLOCKING",
                Columns = ["id", "uuid.short", "start.active", "depends", "project", "tags", "recur", "wait", "scheduled.remaining", "due.relative", "until.remaining", "description.count", "urgency"],
                Labels = ["ID", "UUID", "A", "Deps", "Project", "Tags", "R", "W", "Sch", "Due", "Until", "Description", "Urg"],
            });
            reportDict.Add("report.completed", new ReportDefinition()
            {
                Description = "Completed tasks",
                Filter = "status:completed",
                Columns = ["id", "uuid.short", "entry", "end", "entry.age", "depends", "priority", "project", "tags", "recur.indicator", "due", "description"],
                Labels = ["ID", "UUID", "Created", "Completed", "Age", "Deps", "P", "Project", "Tags", "R", "Due", "Description"],
            });
            reportDict.Add("report.list", new ReportDefinition()
            {
                Description = "Most details of tasks",
                Filter = "status:pending -WAITING",
                Columns = ["id", "start.age", "entry.age", "depends.indicator", "priority", "project", "tags", "recur.indicator", "scheduled.countdown", "due", "until.remaining", "description", "urgency"],
                Labels = ["ID", "Active", "Age", "D", "P", "Project", "Tags", "R", "Sch", "Due", "Until", "Description", "Urg"],
            });
            reportDict.Add("report.long", new ReportDefinition()
            {
                Description = "All details of tasks",
                Filter = "status:pending -WAITING",
                Columns = ["id", "start.active", "entry", "modified.age", "depends", "priority", "project", "tags", "recur", "wait.remaining", "scheduled", "due", "until", "description"],
                Labels = ["ID", "A", "Created", "Mod", "Deps", "P", "Project", "Tags", "Recur", "Wait", "Sched", "Due", "Until", "Description"],
            });
            reportDict.Add("report.ls", new ReportDefinition()
            {
                Description = "Few details of tasks",
                Filter = "status:pending -WAITING",
                Columns = ["id", "start.active", "depends.indicator", "project", "tags", "recur.indicator", "wait.remaining", "scheduled.countdown", "due.countdown", "until.countdown", "description.count"],
                Labels = ["ID", "A", "D", "Project", "Tags", "R", "Wait", "S", "Due", "Until", "Description"],
            });
            reportDict.Add("report.minimal", new ReportDefinition()
            {
                Description = "Minimal details of tasks",
                Filter = "status:pending",
                Columns = ["id", "project", "tags.count", "description.count"],
                Labels = ["ID", "Project", "Tags", "Description"],
            });
            reportDict.Add("report.newest", new ReportDefinition()
            {
                Description = "Newest tasks",
                Filter = "status:pending",
                Columns = ["id", "start.age", "entry", "entry.age", "modified.age", "depends.indicator", "priority", "project", "tags", "recur.indicator", "wait.remaining", "scheduled.countdown", "due", "until.age", "description"],
                Labels = ["ID", "Active", "Created", "Age", "Mod", "D", "P", "Project", "Tags", "R", "Wait", "Sch", "Due", "Until", "Description"],
            });
            reportDict.Add("report.next", new ReportDefinition()
            {
                Description = "Most urgent tasks",
                Filter = "status:pending -WAITING limit:page",
                Columns = ["id", "start.age", "entry.age", "depends", "priority", "project", "tags", "recur", "scheduled.countdown", "due.relative", "until.remaining", "description", "urgency"],
                Labels = ["ID", "Active", "Age", "Deps", "P", "Project", "Tag", "Recur", "S", "Due", "Until", "Description", "Urg"],
            });
            reportDict.Add("report.oldest", new ReportDefinition()
            {
                Description = "Oldest tasks",
                Filter = "status:pending",
                Columns = ["id", "start.age", "entry", "entry.age", "modified.age", "depends.indicator", "priority", "project", "tags", "recur.indicator", "wait.remaining", "scheduled.countdown", "due", "until.age", "description"],
                Labels = ["ID", "Active", "Created", "Age", "Mod", "D", "P", "Project", "Tags", "R", "Wait", "Sch", "Due", "Until", "Description"],
            });
            reportDict.Add("report.overdue", new ReportDefinition()
            {
                Description = "Overdue tasks",
                Filter = "status:pending and +OVERDUE",
                Columns = ["id", "start.age", "entry.age", "depends", "priority", "project", "tags", "recur.indicator", "scheduled.countdown", "due", "until", "description", "urgency"],
                Labels = ["ID", "Active", "Age", "Deps", "P", "Project", "Tag", "R", "S", "Due", "Until", "Description", "Urg"],
            });
            reportDict.Add("report.ready", new ReportDefinition()
            {
                Description = "Most urgent actionable tasks",
                Filter = "+READY",
                Columns = ["id", "start.age", "entry.age", "depends.indicator", "priority", "project", "tags", "recur.indicator", "scheduled.countdown", "due.countdown", "until.remaining", "description", "urgency"],
                Labels = ["ID", "Active", "Age", "D", "P", "Project", "Tags", "R", "S", "Due", "Until", "Description", "Urg"],
            });
            reportDict.Add("report.recurring", new ReportDefinition()
            {
                Description = " Tasks",
                Filter = "status:pending and (+PARENT or +CHILD)",
                Columns = ["id", "start.age", "entry.age", "depends.indicator", "priority", "project", "tags", "recur", "scheduled.countdown", "due", "until.remaining", "description", "urgency"],
                Labels = ["ID", "Active", "Age", "D", "P", "Project", "Tags", "Recur", "Sch", "Due", "Until", "Description", "Urg"],
            });
            reportDict.Add("report.unblocked", new ReportDefinition()
            {
                Description = " tasks",
                Filter = "status:pending -WAITING -BLOCKED",
                Columns = ["id", "depends", "project", "priority", "due", "start.active", "entry.age", "description"],
                Labels = ["ID", "Deps", "Proj", "Pri", "Due", "Active", "Age", "Description"],
            });
            reportDict.Add("report.waiting", new ReportDefinition()
            {
                Description = "Waiting (hidden) tasks",
                Filter = "+WAITING",
                Columns = ["id", "start.active", "entry.age", "depends.indicator", "priority", "project", "tags", "recur.indicator", "wait", "wait.remaining", "scheduled", "due", "until", "description"],
                Labels = ["ID", "A", "Age", "D", "P", "Project", "Tags", "R", "Wait", "Remaining", "Sched", "Due", "Until", "Description"],
            });

            return reportDict;
        }
        else
        {
            var reportDict = new ReportDictionary();

            reportDict.Add("report.active", new ReportDefinition()
            {
                Description = "Active tasks",
                Filter = "status:pending and +ACTIVE",
                Columns = ["id", "start", "start.age", "entry.age", "depends.indicator", "priority", "project", "tags", "recur", "wait", "scheduled.remaining", "due", "until", "description"],
                Labels = ["ID", "Started", "Active", "Age", "D", "P", "Project", "Tags", "Recur", "W", "Sch", "Due", "Until", "Description"],
            });

            reportDict.Add("report.all", new ReportDefinition()
            {
                Description = "All tasks",
                Filter = "",
                Columns = ["id", "status.short", "uuid.short", "start.active", "entry.age", "end.age", "depends.indicator", "priority", "project.parent", "tags.count", "recur.indicator", "wait.remaining", "scheduled.remaining", "due", "until.remaining", "description"],
                Labels = ["ID", "St", "UUID", "A", "Age", "Done", "D", "P", "Project", "Tags", "R", "Wait", "Sch", "Due", "Until", "Description"],
            });
            reportDict.Add("report.blocked", new ReportDefinition()
            {
                Description = "Blocked tasks",
                Filter = "status:pending -WAITING +BLOCKED",
                Columns = ["id", "depends", "project", "priority", "due", "start.active", "entry.age", "description"],
                Labels = ["ID", "Deps", "Proj", "Pri", "Due", "Active", "Age", "Description"],
            });
            reportDict.Add("report.blocking", new ReportDefinition()
            {
                Description = "Blocking tasks",
                Filter = "status:pending -WAITING +BLOCKING",
                Columns = ["id", "uuid.short", "start.active", "depends", "project", "tags", "recur", "wait", "scheduled.remaining", "due.relative", "until.remaining", "description.count", "urgency"],
                Labels = ["ID", "UUID", "A", "Deps", "Project", "Tags", "R", "W", "Sch", "Due", "Until", "Description", "Urg"],
            });
            reportDict.Add("report.completed", new ReportDefinition()
            {
                Description = "Completed tasks",
                Filter = "status:completed",
                Columns = ["id", "uuid.short", "entry", "end", "entry.age", "depends", "priority", "project", "tags", "recur.indicator", "due", "description"],
                Labels = ["ID", "UUID", "Created", "Completed", "Age", "Deps", "P", "Project", "Tags", "R", "Due", "Description"],
            });
            reportDict.Add("report.list", new ReportDefinition()
            {
                Description = "Most details of tasks",
                Filter = "status:pending -WAITING",
                Columns = ["id", "start.age", "entry.age", "depends.indicator", "priority", "project", "tags", "recur.indicator", "scheduled.countdown", "due", "until.remaining", "description", "urgency"],
                Labels = ["ID", "Active", "Age", "D", "P", "Project", "Tags", "R", "Sch", "Due", "Until", "Description", "Urg"],
            });
            reportDict.Add("report.long", new ReportDefinition()
            {
                Description = "All details of tasks",
                Filter = "status:pending -WAITING",
                Columns = ["id", "start.active", "entry", "modified.age", "depends", "priority", "project", "tags", "recur", "wait.remaining", "scheduled", "due", "until", "description"],
                Labels = ["ID", "A", "Created", "Mod", "Deps", "P", "Project", "Tags", "Recur", "Wait", "Sched", "Due", "Until", "Description"],
            });
            reportDict.Add("report.ls", new ReportDefinition()
            {
                Description = "Few details of tasks",
                Filter = "status:pending -WAITING",
                Columns = ["id", "start.active", "depends.indicator", "project", "tags", "recur.indicator", "wait.remaining", "scheduled.countdown", "due.countdown", "until.countdown", "description.count"],
                Labels = ["ID", "A", "D", "Project", "Tags", "R", "Wait", "S", "Due", "Until", "Description"],
            });
            reportDict.Add("report.minimal", new ReportDefinition()
            {
                Description = "Minimal details of tasks",
                Filter = "status:pending",
                Columns = ["id", "project", "tags.count", "description.count"],
                Labels = ["ID", "Project", "Tags", "Description"],
            });
            reportDict.Add("report.newest", new ReportDefinition()
            {
                Description = "Newest tasks",
                Filter = "status:pending",
                Columns = ["id", "start.age", "entry", "entry.age", "modified.age", "depends.indicator", "priority", "project", "tags", "recur.indicator", "wait.remaining", "scheduled.countdown", "due", "until.age", "description"],
                Labels = ["ID", "Active", "Created", "Age", "Mod", "D", "P", "Project", "Tags", "R", "Wait", "Sch", "Due", "Until", "Description"],
            });
            reportDict.Add("report.next", new ReportDefinition()
            {
                Description = "Most urgent tasks",
                Filter = "status:pending -WAITING limit:page",
                Columns = ["id", "start.age", "entry.age", "depends", "priority", "project", "tags", "recur", "scheduled.countdown", "due.relative", "until.remaining", "description", "urgency"],
                Labels = ["ID", "Active", "Age", "Deps", "P", "Project", "Tag", "Recur", "S", "Due", "Until", "Description", "Urg"],
            });
            reportDict.Add("report.oldest", new ReportDefinition()
            {
                Description = "Oldest tasks",
                Filter = "status:pending",
                Columns = ["id", "start.age", "entry", "entry.age", "modified.age", "depends.indicator", "priority", "project", "tags", "recur.indicator", "wait.remaining", "scheduled.countdown", "due", "until.age", "description"],
                Labels = ["ID", "Active", "Created", "Age", "Mod", "D", "P", "Project", "Tags", "R", "Wait", "Sch", "Due", "Until", "Description"],
            });
            reportDict.Add("report.overdue", new ReportDefinition()
            {
                Description = "Overdue tasks",
                Filter = "status:pending and +OVERDUE",
                Columns = ["id", "start.age", "entry.age", "depends", "priority", "project", "tags", "recur.indicator", "scheduled.countdown", "due", "until", "description", "urgency"],
                Labels = ["ID", "Active", "Age", "Deps", "P", "Project", "Tag", "R", "S", "Due", "Until", "Description", "Urg"],
            });
            reportDict.Add("report.ready", new ReportDefinition()
            {
                Description = "Most urgent actionable tasks",
                Filter = "+READY",
                Columns = ["id", "start.age", "entry.age", "depends.indicator", "priority", "project", "tags", "recur.indicator", "scheduled.countdown", "due.countdown", "until.remaining", "description", "urgency"],
                Labels = ["ID", "Active", "Age", "D", "P", "Project", "Tags", "R", "S", "Due", "Until", "Description", "Urg"],
            });
            reportDict.Add("report.recurring", new ReportDefinition()
            {
                Description = " Tasks",
                Filter = "status:pending and (+PARENT or +CHILD)",
                Columns = ["id", "start.age", "entry.age", "depends.indicator", "priority", "project", "tags", "recur", "scheduled.countdown", "due", "until.remaining", "description", "urgency"],
                Labels = ["ID", "Active", "Age", "D", "P", "Project", "Tags", "Recur", "Sch", "Due", "Until", "Description", "Urg"],
            });
            reportDict.Add("report.unblocked", new ReportDefinition()
            {
                Description = " tasks",
                Filter = "status:pending -WAITING -BLOCKED",
                Columns = ["id", "depends", "project", "priority", "due", "start.active", "entry.age", "description"],
                Labels = ["ID", "Deps", "Proj", "Pri", "Due", "Active", "Age", "Description"],
            });
            reportDict.Add("report.waiting", new ReportDefinition()
            {
                Description = "Waiting (hidden) tasks",
                Filter = "+WAITING",
                Columns = ["id", "start.active", "entry.age", "depends.indicator", "priority", "project", "tags", "recur.indicator", "wait", "wait.remaining", "scheduled", "due", "until", "description"],
                Labels = ["ID", "A", "Age", "D", "P", "Project", "Tags", "R", "Wait", "Remaining", "Sched", "Due", "Until", "Description"],
            });

            return reportDict;
        }
    }
}

// [TomlSerializedObject]
public partial class ReportDictionary : IDictionary<string, ReportDefinition>
{
    private readonly Dictionary<string, ReportDefinition> _backingDict = [];
    public ReportDefinition this[string key]
    {
        get => _backingDict[key]; set
        {
            _backingDict[key] = value;
            SetReportName(key, value);
        }
    }

    public ICollection<string> Keys => _backingDict.Keys;
    public ICollection<ReportDefinition> Values => _backingDict.Values;
    public int Count => _backingDict.Count;
    public bool IsReadOnly { get; } = false;

    public void Add(string key, ReportDefinition value)
    {
        _backingDict.Add(key, value);
        SetReportName(key, value);
    }

    public void Add(KeyValuePair<string, ReportDefinition> item)
    {
        _backingDict.Add(item.Key, item.Value);
        SetReportName(item.Key, item.Value);
    }

    public void Clear() => _backingDict.Clear();

    public bool Contains(KeyValuePair<string, ReportDefinition> item) => _backingDict.Contains(item);

    public bool ContainsKey(string key) => _backingDict.ContainsKey(key);

    public void CopyTo(KeyValuePair<string, ReportDefinition>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, ReportDefinition>> GetEnumerator()
    {
        return _backingDict.GetEnumerator();
    }

    public bool Remove(string key)
    {
        return _backingDict.Remove(key);
    }

    public bool Remove(KeyValuePair<string, ReportDefinition> item)
    {
        return _backingDict.Remove(item.Key, out var value);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out ReportDefinition value)
    {
        return _backingDict.TryGetValue(key, out value);
    }

    private static void SetReportName(string key, ReportDefinition value)
    {
        value.Name = key;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return _backingDict.GetEnumerator();
    }
}
public interface IConfig
{
    [TomlValueOnSerialized] public string Name { get; set; }
}

[TomlSerializedObject]
public partial class ReportDefinition : IConfig
{
    [TomlValueOnSerialized] public string Name { get; set; } = default!;
    [TomlValueOnSerialized] public required string Description { get; set; }
    [TomlValueOnSerialized] public string Filter { get; set; } = string.Empty;
    [TomlValueOnSerialized] public string[] Columns { get; set; } = [];
    [TomlValueOnSerialized] public string[] Labels { get; set; } = [];

    public static ReportDefinition FromFilter(string v)
    {
        throw new NotImplementedException();
    }

    public ReportDefinition OverrideFilter(params string[] filter)
    {
        Filter = string.Join(' ', filter);

        return this;
    }
    // TODO: Add support for sorting
}

public class ReportDictionaryFormatter : ITomlValueFormatter<IDictionary<string, ReportDefinition>>
{
    public IDictionary<string, ReportDefinition> Deserialize(ref TomlDocumentNode rootNode, CsTomlSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public void Serialize<TBufferWriter>(ref Utf8TomlDocumentWriter<TBufferWriter> writer, IDictionary<string, ReportDefinition> target, CsTomlSerializerOptions options) where TBufferWriter : System.Buffers.IBufferWriter<byte>
    {
        writer.BeginScope();
        if (options.SerializeOptions.TableStyle == TomlTableStyle.Header && (writer.State == TomlValueState.Default || writer.State == TomlValueState.Table))
        {
            writer.WriteTableHeader("Test"u8);
            writer.WriteNewLine();
            // writer.BeginCurrentState(TomlValueState.Table);
            // writer.PushKey("Report1"u8);
            // options.Resolver.GetFormatter<IDictionary<string, ReportDefinition>>()!.Serialize(ref writer, target, options);
            // writer.PopKey();
            writer.EndCurrentState();
        }
        else
        {
            writer.PushKey("Test2"u8);
            // options.Resolver.GetFormatter<IDictionary<string, ReportDefinition>>()!.Serialize(ref writer, target, options);
            writer.PopKey();
        }
        writer.EndScope();
    }
}
