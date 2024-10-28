using System.Reflection;
using System.Runtime.CompilerServices;

using TaskTitan.Data;
using TaskTitan.Data.Reports;

using static TaskTitan.Data.Enums;

namespace TaskTitan.Cli.AnsiConsole;

public class ReportWriter : IReportWriter
{
    private readonly IEnumerable<PropertyInfo> _typeProperties = typeof(TaskItem).GetProperties();
    public Task<int> Display(CustomReport report, IEnumerable<TaskItem> tasks)
    {
        if (report.Columns.Length != report.Labels.Length) throw new Exception("Mismatched column and label counts");

        var grid = new Grid();

        grid.AddColumns(report.Labels.Length);
        grid.AddRow(report.Labels);
        string[] rowVals = new string[report.Columns.Length];
        foreach (var task in tasks)
        {
            for (int i = 0; i < report.Columns.Length; i++)
            {
                var split = report.Columns[i].Split('.');
                (string field, ColFormat? format) items = split switch
                {
                    { Length: 1 } => (split[0], null),
                    [_, "age"] => (split[0], ColFormat.Age),
                    [_, "indicator"] => (split[0], ColFormat.Indicator),
                    [_, "countdown"] => (split[0], ColFormat.Countdown),
                    [_, "remaining"] => (split[0], ColFormat.Countdown),
                    _ => throw new SwitchExpressionException($"{report.Columns[i]}")
                };

                var curentProp = _typeProperties
                    .SingleOrDefault(p => p.Name.StartsWith(items.field, StringComparison.OrdinalIgnoreCase));

                if (curentProp is null) Console.WriteLine(items.field);
                if (curentProp.PropertyType == typeof(DateTime))
                {
                    if (curentProp.GetValue(task) is not DateTime currentVal)
                    {
                        rowVals[i] = string.Empty;
                        continue;
                    }

                    rowVals[i] = items.format switch
                    {
                        null => currentVal.ToString() ?? string.Empty,
                        ColFormat.Age => (DateTime.Now - currentVal).ToString(),
                        ColFormat.Countdown => (currentVal - DateTime.Now).ToString(),
                        _ => throw new SwitchExpressionException($"Unsupported format {items.format}")
                    };
                }
                else if (curentProp.PropertyType == typeof(string))
                {
                    if (curentProp.GetValue(task) is not string currentVal)
                    {
                        rowVals[i] = string.Empty;
                        continue;
                    }

                    rowVals[i] = items.format switch
                    {
                        null => currentVal ?? string.Empty,
                        ColFormat.Indicator => curentProp.Name[0].ToString(),
                        _ => throw new SwitchExpressionException($"Unsupported format {items.format}")
                    };
                }
                else if (curentProp.PropertyType == typeof(double) || curentProp.PropertyType == typeof(int))
                {
                    double currentVal = Convert.ToDouble(curentProp.GetValue(task));

                    rowVals[i] = items.format switch
                    {
                        null => currentVal.ToString(),
                        ColFormat.Indicator => curentProp.Name[0].ToString(),
                        // _ => throw new SwitchExpressionException($"Unsupported format {items.format}")
                        _ => currentVal.ToString()

                    };
                }
                else if (curentProp.PropertyType == typeof(string[]))
                {
                    string[] currentVal = curentProp.GetValue(task) as string[] ?? [];

                    if (currentVal.Length == 0)
                    {
                        rowVals[i] = "";
                    }
                    else
                    {
                        rowVals[i] = $"{string.Join(",", currentVal)}";
                    }
                }
                rowVals[i] = rowVals[i] ?? string.Empty;
            }
            grid.AddRow(rowVals);
        }

        Spectre.Console.AnsiConsole.Write(grid);

        return Task.FromResult(0);
    }
}

public interface IReportWriter
{
    Task<int> Display(CustomReport report, IEnumerable<TaskItem> tasks);
}
