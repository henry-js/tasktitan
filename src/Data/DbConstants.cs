using TaskTitan.Core.Enums;

namespace TaskTitan.Data;

public static class Constants
{
    public const string TasksDbConnectionString = "TasksDb";

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
        public static readonly string CreateTaskModifiedTrigger = $"""
CREATE TRIGGER tasks_on_update
AFTER UPDATE ON tasks
FOR EACH ROW
BEGIN
    UPDATE tasks SET
        {TaskItemAttribute.Modified} = CURRENT_TIMESTAMP
    WHERE {TaskItemAttribute.Id} = NEW.{TaskItemAttribute.Id};
END;

""";

        public static readonly string DropTaskModifiedTrigger = "DROP TRIGGER IF EXISTS tasks_on_update";

        public static readonly string CreateTaskCreatedTrigger = $"""
CREATE TRIGGER tasks_on_insert
AFTER INSERT ON tasks
FOR EACH ROW
BEGIN
    UPDATE tasks SET
        {TaskItemAttribute.Modified} = CURRENT_TIMESTAMP,
        {TaskItemAttribute.Entry} = CURRENT_TIMESTAMP
    WHERE {TaskItemAttribute.Id} = NEW.{TaskItemAttribute.Id};
END;
""";
        public static readonly string DropTaskCreatedTrigger = "DROP TRIGGER IF EXISTS tasks_on_insert";
    }
}
