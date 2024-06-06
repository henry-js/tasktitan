namespace TaskTitan.Data;

public static class DbConstants
{
    public static class KeyWords
    {
        public const string Between = "BETWEEN";
        public const string In = "IN";
        public const string And = "AND";
    }
    public static class TasksTable
    {
        public const string Name = "tasks";
        public const string RowId = "RowId";
        public const string TasksQuery = $"""
SELECT * FROM {Name}
""";
        public const string TasksWithRowId = "tasks_with_rowId"
        ; public const string CreateViewTasksWithRowId = $"""
CREATE VIEW {TasksWithRowId} as
SELECT
    *,
    row_number() OVER ( ORDER BY {nameof(TaskItem.Created)}) RowId
FROM tasks
""";
        public const string DropViewTasksWithRowId = $"DROP VIEW {TasksWithRowId}";
    }
}
