using System.Reflection;

using TaskTitan.Core.Enums;

namespace TaskTitan.Infrastructure.Services;

public record TaskItemCreateRequest : ITaskRequest
{
    public string[] Filters { get; init; } = [];
    public Action Operation { get; } = Action.Create;

    public required TaskItem Task { get; set; }
}

public record TaskItemGetRequest : ITaskRequest
{
    public string[] Filters { get; init; } = [];
    public IEnumerable<FormattedTaskItemAttribute> Fields { get; init; } = [];
    public Action Operation { get; } = Action.Fetch;
}

public record CreateTaskItemDto
{
    public required string Description { get; init; }
    public TaskItemState State { get; init; }
    public string? Due { get; init; }
    public string? Until { get; init; }
    public string? Wait { get; init; }
    public string? Started { get; init; }
    public string? Ended { get; init; }
    public string? Scheduled { get; init; }
}
public class FormattedTaskItemAttribute
{
    private readonly string _col;
    private static readonly PropertyInfo[] Properties = typeof(TaskItem).GetProperties();

    public FormattedTaskItemAttribute(string col)
    {
        var split = col.Split(".");
        FieldName = (TaskItemAttribute)split[0];
        Format = split.Length > 1 ? Enum.Parse<FieldFormat>(split[1], true) : DefaultFormat(FieldName);
        _col = col;
        Property = Properties.SingleOrDefault(p => string.Equals(FieldName, p.Name, StringComparison.OrdinalIgnoreCase));
    }

    private static FieldFormat DefaultFormat(TaskItemAttribute fieldName)
    {
        return fieldName.Value switch
        {
            TaskItemConstants.Field.id => FieldFormat.None,
            TaskItemConstants.Field.description => FieldFormat.None,
            TaskItemConstants.Field.status => FieldFormat.Short,
            TaskItemConstants.Field.project => FieldFormat.None,
            TaskItemConstants.Field.due => FieldFormat.Date,
            TaskItemConstants.Field.until => FieldFormat.Date,
            TaskItemConstants.Field.limit => FieldFormat.None,
            TaskItemConstants.Field.wait => FieldFormat.Date,
            TaskItemConstants.Field.entry => FieldFormat.Age,
            TaskItemConstants.Field.end => FieldFormat.Date,
            TaskItemConstants.Field.start => FieldFormat.Date,
            TaskItemConstants.Field.scheduled => FieldFormat.Date,
            TaskItemConstants.Field.modified => FieldFormat.Date,
            TaskItemConstants.Field.depends => FieldFormat.Date,
            TaskItemConstants.Field.tag => FieldFormat.None,
            _ => FieldFormat.None
        };
    }

    public TaskItemAttribute FieldName { get; }
    public FieldFormat Format { get; }
    public PropertyInfo? Property { get; }
}

public enum FieldFormat
{
    None,
    Long,
    Short,
    Date,
    Age,
    Indicator,
    Countdown,
    Count,
    Parent,
    Remaining
}
