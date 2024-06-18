using System.Diagnostics;
using System.Threading.Tasks;

using TaskTitan.Core.Enums;
using TaskTitan.Lib.Dtos;

namespace TaskTitan.Cli.TaskCommands;

// TODO: Should use a filter to LIST commands first then perform modification
internal sealed class ModifyCommand(IAnsiConsole console, IStringFilterConverter<DateTime> dateConverter, ITaskItemService service, ILogger<ModifyCommand> logger)
: AsyncCommand<ModifySettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ModifySettings settings)
    {
        Dictionary<TaskItemAttribute, string> modifiers = [];


        TaskItemModifyRequest request = new()
        {
            Filters = settings.filterText ?? [],
        };
        if (settings.Description is not null)
            request.Attributes.Add(TaskItemAttribute.Description, string.Join(" ", settings.Description));
        if (settings.due is not null)
            request.Attributes.Add(TaskItemAttribute.Due, settings.due);
        if (settings.scheduled is not null)
            request.Attributes.Add(TaskItemAttribute.Scheduled, settings.scheduled);
        if (settings.until is not null)
            request.Attributes.Add(TaskItemAttribute.Until, settings.until);
        if (settings.wait is not null)
            request.Attributes.Add(TaskItemAttribute.Wait, settings.wait);

        await service.Update(request);
        return 0;
    }

    private TaskItemModifyDto SettingsToDto(ModifySettings settings)
    {
        TaskItemModifyDto dto = new()
        {
            Description = string.Join(' ', settings.Description),
            DueText = settings.due,
            ScheduledText = settings.scheduled,
            WaitText = settings.wait,
            UntilText = settings.until,
        };

        return dto;

    }
}
