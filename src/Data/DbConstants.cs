using TaskTitan.Core.Enums;

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
    row_number() OVER ( ORDER BY {nameof(TaskItem.Entry)}) RowId
FROM tasks
""";
        public const string DropViewTasksWithRowId = $"DROP VIEW {TasksWithRowId}";
        public static readonly string ModifiedTrigger = $"""
CREATE TRIGGER tasks_update_modified
AFTER UPDATE ON tasks
FOR EACH ROW
BEGIN
    UPDATE tasks SET {TaskItemAttribute.Modified}
""";
    }
}
