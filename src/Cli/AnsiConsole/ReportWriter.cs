
using TaskTitan.Data;
using TaskTitan.Data.Reports;

namespace TaskTitan.Cli.AnsiConsole;

public class ReportWriter : IReportWriter
{
    public Task<int> Display(CustomReport report, IEnumerable<TaskItem> tasks)
    {
        if (report.Columns.Length != report.Labels.Length) throw new Exception("Mismatched column and label counts");

        var grid = new Grid();

        grid.AddColumns(report.Labels.Length);
        grid.AddRow(report.Labels);
        foreach (var task in tasks)
        {

        }

        return Task.FromResult(0);
    }
}

public interface IReportWriter
{
    Task<int> Display(CustomReport report, IEnumerable<TaskItem> tasks);
}
