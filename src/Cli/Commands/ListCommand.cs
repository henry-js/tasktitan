using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Spectre.Console;

using System.CommandLine;
using System.CommandLine.Invocation;

using TaskTitan.Configuration;
using TaskTitan.Data;
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Extensions;
using TaskTitan.Data.Parsers;
using TaskTitan.Data.Reports;

namespace TaskTitan.Cli.Commands;

public sealed class ListCommand : Command
{
    public ListCommand(ReportDictionary? reports = null) : base("list", "Add a task to the list")
    {
        AddOptions(this, reports);
    }

    public static void AddOptions(Command command, ReportDictionary? reports)
    {
        Option<FilterExpression?> option = new(
            aliases: ["-f", "--filter"],
            description: "Filter tasks by",
            parseArgument: ar =>
            {
                return ExpressionParser.ParseFilter(string.Join(' ', ar.Tokens));
            })
        {
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.ZeroOrMore,
        };
        command.AddOption(option);

        Argument<CustomReport?> report = new(
            name: "Report",
            description: "Use a report instead of filter",
            parse: ar => reports?.TryGetValue(ar.Tokens.FirstOrDefault()!.Value, out var report) == true ? report : null
        )
        {
            Arity = ArgumentArity.ZeroOrOne
        };
        command.AddArgument(report);
    }

    new public class Handler(IAnsiConsole console, LiteDbContext dbContext, ILogger<ListCommand> logger, IOptions<ReportConfiguration> reportOptions) : ICommandHandler
    {
        private readonly ReportConfiguration reportConfig = reportOptions.Value;
        public FilterExpression? Filter { get; set; }
        public CustomReport? Report { get; set; }
        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            Report = Report ?? reportConfig.Report["list"];

            string linqFilterText = Report switch
            {
                not null => ExpressionParser.ParseFilter(Report.Filter).ToBsonExpression(),
                _ => (Filter is not null) ? Filter.ToBsonExpression() : ""
            };

            logger.LogInformation("Information logged");
            console.WriteLine("Hello from list command");

            var tasks = dbContext.ListFromFilter(linqFilterText, true);

            console.MarkupLineInterpolated($"[yellow]Fetced {tasks.Count()} tasks");

            return await Task.FromResult(0);
        }
    }
}
