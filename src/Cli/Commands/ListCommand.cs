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
    public ListCommand() : base("list", "Display a report or tasks filtered")
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
        command.AddOption(new Option<bool>(["-s", "--skip"]) { IsHidden = true });
    }

    new public class Handler(LiteDbContext dbContext, IAnsiConsole console, ILogger<ListCommand> logger, IOptions<TaskTitanConfig> reportOptions) : ICommandHandler
    {
        private readonly TaskTitanConfig appConfig = reportOptions.Value;
        public string[]? Filter { get; set; }
        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;
        public bool Skip { get; set; }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var reportDef = Filter switch
            {
                null or { Length: 0 } => appConfig.Report["list"],
                { Length: 1 } => appConfig.Report.TryGetValue(Filter[0], out var value) ? value : appConfig.Report["list"].OverrideFilter(Filter),
                _ => appConfig.Report["list"].OverrideFilter(Filter)
            };

            logger.LogInformation("Report: {ReportName}, Filter : {ReportFilter}", reportDef.Name, reportDef.Filter);

            FilterExpression query;
            using (logger.TimeOperation("Parsing {reportName} report filter", reportDef.Name))
            {
                query = ExpressionParser.ParseFilter(reportDef.Filter);
            }

            IEnumerable<TaskItem> tasks;

            if (Skip)
            {
                logger.LogInformation("Skipping execution");
                return await Task.FromResult(0);
            }
            using (logger.TimeOperation("Fetching tasks"))
            {
                tasks = dbContext.QueryTasks(query).ToList();
            }

            using (logger.TimeOperation("Generating {reportName} report", reportDef.Name))
            {
                var report = new Report(reportDef, appConfig.Uda);

                var grid = report.Build(tasks);

                console.Write(grid);

                return await Task.FromResult(0);
            }
        }
    }
}
