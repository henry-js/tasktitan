using System.Diagnostics;
using System.Threading.Tasks;

using TaskTitan.Lib.Dates;
using TaskTitan.Lib.Expressions;

namespace TaskTitan.Cli.TaskCommands;

// TODO: Should use a filter to LIST commands first then perform modification
internal sealed class ModifyCommand(IAnsiConsole console, IExpressionParser expressionParser, IStringFilterConverter<DateTime> dateConverter, ITaskItemService service, ILogger<ModifyCommand> logger)
: AsyncCommand<ModifySettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ModifySettings settings)
    {
        IEnumerable<Expression> filters = [];
        if (settings.filterText is not null)
        {
            filters = settings.filterText.Select(f => expressionParser.ParseFilter(f));
        }
        var tasks = await service.GetTasks(filters);
        logger.LogDebug(tasks.Count() + " tasks found");
        // ParsedInput input = ParseInput(settings);
        if (!tasks.Any())
        {
            return -1;
        }
        if (settings.due != null)
        {
            foreach (var task in tasks)
            {
                task.Due = dateConverter.ConvertFrom(settings.due);
                var updateResult = await service.Update(task);
                Debug.WriteLine(task);
                console.WriteLine(updateResult.IsSuccess ? "Update successful" : $"Update failed");
                if (updateResult.ErrorMessages.Length > 0)
                {
                    console.WriteLine($@"
Errors:
    {string.Join(Environment.NewLine, updateResult.ErrorMessages)}
");
                }
            }
        }

        return 0;
    }
}
