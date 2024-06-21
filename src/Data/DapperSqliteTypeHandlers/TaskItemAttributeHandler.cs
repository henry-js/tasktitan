using System.Data;

using Dapper;

using TaskTitan.Core.Enums;

namespace TaskTitan.Data.DapperSqliteTypeHandlers;

internal class TaskItemAttributeHandler : SqlMapper.TypeHandler<TaskItemAttribute>
{
    public override TaskItemAttribute Parse(object value)
    {
        return Enum.TryParse<TaskItemAttribute>(value.ToString(), out var result) ? result : TaskItemAttribute.Empty;
    }

    public override void SetValue(IDbDataParameter parameter, TaskItemAttribute value)
    {
        parameter.Value = value.ToString();
    }
}

internal class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
{
    public override DateTime Parse(object value)
    {
        var parsedVal = DateTime.Parse((string)value);
        return DateTime.SpecifyKind(parsedVal, DateTimeKind.Utc);
    }

    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}

internal class TaskItemStateHandler : SqlMapper.TypeHandler<TaskItemState>
{
    public override TaskItemState Parse(object value)
    {
        return Enum.Parse<TaskItemState>(value.ToString() ?? "");
    }

    public override void SetValue(IDbDataParameter parameter, TaskItemState value)
    {
        parameter.Value = Enum.GetName(typeof(TaskItemState), value);
    }
}
