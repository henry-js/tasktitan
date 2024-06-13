using System.Data;

using Dapper;

using TaskTitan.Core.Enums;

namespace TaskTitan.Data.DapperSqliteTypeHandlers;

internal class TaskItemAttributeHandler : SqliteTypeHandler<TaskItemAttribute>
{
    public override TaskItemAttribute Parse(object value)
    {
        return Enum.TryParse<TaskItemAttribute>(value.ToString(), out var result) ? result : TaskItemAttribute.Empty;
    }

    public void SetValue(IDbDataParameter parameter, object value)
    {

    }
}
