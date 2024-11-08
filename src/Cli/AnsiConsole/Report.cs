using LiteDB;

using TaskTitan.Data.Reports;

namespace TaskTitan.Cli.AnsiConsole;

public class Report
{
    private readonly ReportDefinition _def;

    public Report(ReportDefinition definition)
    {
        _def = definition;
    }

    public Grid Build(IEnumerable<BsonDocument> documents)
    {
        var grid = new Grid();

        grid.AddColumns(_def.Columns.Length);
        grid.AddRow(_def.Labels);

        foreach (var doc in documents)
        {
            foreach (var col in _def.Columns)
            {
            }
        }

        return grid;
    }
}
