using System.Data;
using System.Globalization;

using Dapper;

namespace TaskTitan.Data.DapperSqliteTypeHandlers;

internal class TaskItemIdHandler : SqlMapper.TypeHandler<TaskItemId>
{
    public override TaskItemId Parse(object value)
    {
        return value is not null ? new TaskItemId(value.ToString()!) : TaskItemId.Empty;
    }

    public override void SetValue(IDbDataParameter parameter, TaskItemId value)
    {
        parameter.Value = value.ToString();
    }
}

internal class TaskDateHandler : SqlMapper.TypeHandler<TaskDate>
{
    public override TaskDate Parse(object value)
    {
        return DateTime.Parse((string)value, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);
    }

    public override void SetValue(IDbDataParameter parameter, TaskDate value)
    {
        parameter.Value = value.ToString();
    }
}
