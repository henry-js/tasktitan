
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

using LiteDB;

using Spectre.Console;
using Spectre.Console.Rendering;

using TaskTitan.Core;
using TaskTitan.Core.Enums;
using TaskTitan.Data.Reports;

namespace TaskTitan.Cli.Display;

public class Report
{
    private readonly Dictionary<string, ColFormatDefinition?> _columns = [];
    private readonly IEnumerable<PropertyInfo> _typeProperties = typeof(TaskItem).GetProperties();

    public Report(ReportDefinition def)
    {
        _columns = [];
        for (int i = 0; i < def.Columns.Length; i++)
        {
            string? col = def.Columns[i];
            var split = col.Split('.');
            KeyValuePair<string, ColFormatDefinition?> formatDef = split switch
            {
                { Length: 1 } => new(split[0], null),
                [_, "age"] => new(split[0], new(def.Labels[i], ColFormat.Age)),
                [_, "indicator"] => new(split[0], new(def.Labels[i], ColFormat.Indicator)),
                [_, "countdown"] => new(split[0], new(def.Labels[i], ColFormat.Countdown)),
                [_, "remaining"] => new(split[0], new(def.Labels[i], ColFormat.Countdown)),
                _ => throw new SwitchExpressionException($"{col}")
            };
            _columns.TryAdd(formatDef.Key, formatDef.Value);
        }
    }

    public Grid Build(IEnumerable<TaskItem> tasks)
    {
        var props = typeof(TaskItem).GetProperties();

        var grid = new Grid();

        grid.AddColumns(_columns.Keys.Count);
        grid.AddRow(_columns.Values.Select(x => x.Label).ToArray());

        foreach (var task in tasks)
        {
            AddRow(grid, task, props);
        }

        return grid;
    }

    private void AddRow(Grid grid, TaskItem task, PropertyInfo[] props)
    {
        var rowCols = new List<IRenderable>();
        foreach (var col in _columns)
        {
            var currentProp = props.FirstOrDefault(p => p.Name.StartsWith(col.Key, StringComparison.InvariantCultureIgnoreCase));
            var stringVal = ParsePropertyValue(currentProp, task, col.Value.Format);
            rowCols.Add(stringVal);
        }

        grid.AddRow(rowCols.ToArray());
    }

    private Text ParsePropertyValue(PropertyInfo v, TaskItem task, ColFormat? format)
    {
        if (v is null) return Text.Empty;
        var val = v.GetValue(task);

        return val switch
        {
            null => Text.Empty,
            string s => new Text(FormattedString(s, format)),
            DateTime dt => new Text(FormattedDate(dt, format)),
            double num => new Text(FormattedNumber(num, format)),
            string[] arr => new Text(FormattedArray(arr, format)),
            _ => throw new SwitchExpressionException($"{val}"),
        };

        string FormattedString(string s, ColFormat? format)
        {
            throw new NotImplementedException();
        }
        string FormattedDate(DateTime dt, ColFormat? format)
        {
            return format switch
            {
                null or ColFormat.Formatted => dt.ToString("yyyy-MM-dd"),
                ColFormat.Iso => dt.ToString("yyyyMMddTHHmmssZ"),
                ColFormat.Age => (DateTime.Now - dt).ToString("%dday %hhr %mmin")
            };
        }
        string FormattedNumber(double num, ColFormat? format)
        {
            throw new NotImplementedException();
        }
        string FormattedArray(string[] num, ColFormat? format)
        {
            throw new NotImplementedException();
        }
    }

}

internal record ColFormatDefinition(string Label, ColFormat? Format);
