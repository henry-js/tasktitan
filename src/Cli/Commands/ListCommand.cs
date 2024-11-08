using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Ogu.Extensions.Logging.Timings;

using System.CommandLine.Invocation;

using TaskTitan.Cli.Display;
using TaskTitan.Core;
using TaskTitan.Core.Configuration;
using TaskTitan.Data;
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

        command.AddOption(new Option<bool>(["-s", "--skip"]));
    }

    new public class Handler(LiteDbContext dbContext, IReportWriter reportWriter, ILogger<ListCommand> logger, IOptions<TaskTitanConfig> reportOptions) : ICommandHandler
    {
        private readonly TaskTitanConfig reportConfig = reportOptions.Value;
        public string[]? Filter { get; set; }
        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;
        public bool Skip { get; set; } = false;

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var report = Filter switch
            {
                null or { Length: 0 } => reportConfig.Report["list"],
                { Length: 1 } => reportConfig.Report.TryGetValue(Filter[0], out var value) ? value : reportConfig.Report["list"].OverrideFilter(Filter),
                _ => reportConfig.Report["list"].OverrideFilter(Filter)
            };

            logger.LogInformation("Report: {ReportName}, Filter : {ReportFilter}", report.Name, report.Filter);

            FilterExpression query;
            using (logger.TimeOperation("Parsing {reportName} report filter", report.Name))
            {
                query = ExpressionParser.ParseFilter(report.Filter);
            }

            IEnumerable<TaskItem> tasks;

            if (Skip)
            {
                logger.LogInformation("Skipping execution");
                return 0;
            }
            using (logger.TimeOperation("Fetching tasks"))
            {
                tasks = dbContext.QueryTasks(query).ToList();
            }

            using (logger.TimeOperation("Generating {reportName} report", report.Name))
            {
                return await reportWriter.Display(report, tasks);
            }
        }
    }
}
