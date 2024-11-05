using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Ogu.Extensions.Logging.Timings;

using System.CommandLine.Invocation;

using TaskTitan.Cli.AnsiConsole;
using TaskTitan.Configuration;
using TaskTitan.Data;
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Extensions;
using TaskTitan.Data.Parsers;
using TaskTitan.Data.Reports;

namespace TaskTitan.Cli.Commands;

public sealed class ListCommand : Command
{
    public ListCommand(ReportDictionary? reports = null) : base("list", "Display a report or tasks filtered")
    {
        AddOptions(this);
    }

    public static void AddOptions(Command command)
    {
        Argument<string[]?> report = new(
            name: "Filter",
            description: "Use a report instead of filter"
        )
        {
            Arity = ArgumentArity.ZeroOrMore
        };
        command.AddArgument(report);
    }

    new public class Handler(LiteDbContext dbContext, IReportWriter reportWriter, ILogger<ListCommand> logger, IOptions<ReportConfiguration> reportOptions) : ICommandHandler
    {
        private readonly ReportConfiguration reportConfig = reportOptions.Value;
        public string[]? Filter { get; set; }
        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var report = Filter switch
            {
                null or { Length: 0 } => reportConfig.Report["list"],
                { Length: 1 } => reportConfig.Report.TryGetValue(Filter[0], out var value) ? value : reportConfig.Report["list"].OverrideFilter(Filter),
                _ => reportConfig.Report["list"].OverrideFilter(Filter)
            };

            logger.LogInformation("Report: {ReportName}, Filter : {ReportFilter}", report.Name, report.Filter);

            var query = ExpressionParser.ParseFilter(report.Filter).ToBsonExpression();

            IEnumerable<TaskItem> tasks;
            using (logger.TimeOperation("Fetching tasks"))
            {
                tasks = dbContext.QueryTasks(query).ToList();
            }

            return await reportWriter.Display(report, tasks);
        }
    }
}
