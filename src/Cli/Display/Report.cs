using System.Reflection;
using System.Runtime.CompilerServices;

using LiteDB;

using Spectre.Console.Rendering;

using TaskTitan.Core;
using TaskTitan.Core.Configuration;
using TaskTitan.Core.Enums;
using TaskTitan.Data.Reports;

namespace TaskTitan.Cli.Display;

public class Report
{
    private readonly ReportDefinition _def;
    private readonly string[] _labels;
    private readonly Dictionary<string, ColumnDefinition> _columns = [];
    private readonly ConfigDictionary<AttributeDefinition> _udas;
    private readonly PropertyInfo[] _props = typeof(TaskItem).GetProperties();

    public Report(ReportDefinition def, ConfigDictionary<AttributeDefinition> udas)
    {
        _def = def;
        _udas = udas;
        _labels = def.Labels;
        _columns = def.Columns.ToDictionary(k => k.Split('.')[0], e =>
            {
                (string colName, ColFormat colFormat) = e.Split('.') switch
                {
                    { Length: 1 } split => (split[0], ColFormat.Default),
                    [_, "indicator"] split => (split[0], ColFormat.Indicator),
                    [_, "countdown"] split => (split[0], ColFormat.Countdown),
                    [_, "remaining"] split => (split[0], ColFormat.Remaining),
                    [_, "active"] split => (split[0], ColFormat.Active),
                    [_, "short"] split => (split[0], ColFormat.Short),
                    [_, "count"] split => (split[0], ColFormat.Count),
                    [_, _] split => (split[0], Enum.Parse<ColFormat>(split[1], true)),
                    _ => throw new SwitchExpressionException($"{e}")
                };
                if (TaskTitanConfig.DefinedColumns.TryGetValue(colName, out var value))
                    return value.SetFormat(colFormat);
                else if (_udas.TryGetValue(colName, out var attribute))
                    return new ColumnDefinition(attribute, true).SetFormat(colFormat);
                else throw new Exception($"Could not parse report column {e}");
            });
    }

    public Grid Build(IEnumerable<TaskItem> tasks)
    {
        var grid = new Grid();

        grid.AddColumns(_columns.Keys.Count);
        grid.AddRow(_labels);

        foreach (var task in tasks)
        {
            AddRow(grid, task);
        }

        return grid;
    }

    private void AddRow(Grid grid, TaskItem task)
    {
        var rowCols = new List<IRenderable>();
        foreach (var col in _columns)
        {
            var arg = _props
                .First(p => p.Name.StartsWith(col.Key, StringComparison.InvariantCultureIgnoreCase))
                .GetValue(task) ?? string.Empty;

            var text = col.Value.FormatFunc?.Invoke(arg);
            rowCols.Add(text is null ? Text.Empty : new Text(text));
        }

        grid.AddRow([.. rowCols]);
    }
}
