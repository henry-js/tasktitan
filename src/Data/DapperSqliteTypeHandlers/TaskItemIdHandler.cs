namespace TaskTitan.Data.DapperSqliteTypeHandlers;

internal class TaskItemIdHandler : SqliteTypeHandler<TaskItemId>
{
    public override TaskItemId Parse(object value)
    {
        return value is null ? TaskItemId.Empty : new TaskItemId(value.ToString()!);
    }
}
