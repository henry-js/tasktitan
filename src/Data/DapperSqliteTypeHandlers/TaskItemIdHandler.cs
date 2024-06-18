namespace TaskTitan.Data.DapperSqliteTypeHandlers;

internal class TaskItemIdHandler : SqliteTypeHandler<TaskItemId>
{
    public override TaskItemId Parse(object value)
    {
        return value is not null ? new TaskItemId(value.ToString()!) : TaskItemId.Empty;
    }
}
