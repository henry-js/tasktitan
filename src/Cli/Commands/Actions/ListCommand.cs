using Microsoft.Extensions.Options;

using TaskTitan.Cli.Reports;

namespace TaskTitan.Cli.Commands.Actions;

internal sealed class ListCommand : Command
{
    public ListCommand() : base("list", "List tasks in default collection")
    {
        AddOptions(this);
    }

    public static void AddOptions(Command command)
    {
        var filterOption = new Option<string[]?>(
            aliases: ["-f", "--filter"],
            description: "Filter the query");

        command.AddOption(filterOption);
    }

    new public class Handler(IAnsiConsole console, ITaskItemService service, IOptions<Dictionary<ReportOptions.BuiltIn, ReportOptions>> options, ILogger<ListCommand> logger) : ICommandHandler
    {
        public string[] Filter { get; set; } = [];
        public ReportOptions ListReport { get; } = options.Value[ReportOptions.BuiltIn.list];

        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).Result;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var stringSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

            var request = new TaskItemGetRequest()
            {
                Filters = [ListReport.Filter, .. Filter],
                Fields = ListReport.Columns
                    .Split(",", stringSplitOptions)
                    .Select(col => new FormattedTaskItemAttribute(col)).ToList(),
            };

            logger.LogInformation("Handling {Request}", nameof(TaskItemCreateRequest));
            logger.LogInformation("Received filter: {filters}", string.Join(", ", Filter));
            var result = await service.GetTasks(request);

            if (result.IsFailed) return -1;

            var tasks = result.Value;
            if (tasks.Count() == 1)
            {
                console.DisplayTaskDetails(tasks.First());
            }
            else
            {
                console.ListTasks(tasks, request.Fields);
            }

            return 0;
        }
    }
}
