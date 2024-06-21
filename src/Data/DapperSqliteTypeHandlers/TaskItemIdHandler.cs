using System.Data;

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
