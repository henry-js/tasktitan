namespace TaskTitan.Data;

public static class DbConstants
{
    public const string TasksTable = "tasks";
    public const string TasksQuery = $"""
SELECT * FROM {TasksTable}
""";
    public const string TasksWithRowId = "tasks_with_rowId"
    ; public const string CreateViewTasksWithRowId = $"""
CREATE VIEW {TasksWithRowId} as
SELECT
    *,
    row_number() OVER ( ORDER BY {nameof(TaskItem.Created)}) RowId
FROM tasks
""";
}
