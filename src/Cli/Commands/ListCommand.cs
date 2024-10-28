using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        AddOptions(this, reports);
    }

    public static void AddOptions(Command command, ReportDictionary? reports)
    {
        // static FilterExpression? parseArgument(ArgumentResult ar) => ExpressionParser.ParseFilter(string.Join(' ', ar.Tokens));
        // Option<FilterExpression?> option = new(
        //     aliases: ["-f", "--filter"],
        //     description: "Filter tasks by"
        //     // ,parseArgument: parseArgument
        //     )
        // {
        //     AllowMultipleArgumentsPerToken = true,
        //     Arity = ArgumentArity.ZeroOrMore,
        // };
        // command.AddOption(option);

        Argument<string[]?> report = new(
            name: "Filter",
            description: "Use a report instead of filter"
        // , parse: ar => reports?.TryGetValue(ar.Tokens.FirstOrDefault()!.Value, out var report) == true ? report : null
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

            var tasks = dbContext.QueryTasks(query).ToList();

            return await reportWriter.Display(report, tasks);
        }
    }
}
