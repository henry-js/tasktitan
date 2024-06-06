using System.Text;

using TaskTitan.Lib.Expressions;

using static TaskTitan.Data.DbConstants.KeyWords;
using static TaskTitan.Data.DbConstants.TasksTable;

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

public static class ExpressionExtensions
{
    public static string ToQueryString(this IdFilterExpression expression)
    {
        var Ranges = expression.Ranges;
        var Ids = expression.Ids;
        const char separator = ' ';
        int requiredOrKeywords = Ranges.Count() + (Ids.Count > 0 ? 1 : 0) - 1;
        var builder = new StringBuilder();
        if (Ids.Count > 0)
        {
            builder.AppendJoin(separator, RowId, In, $"({Ids})");
            if (requiredOrKeywords > 0)
            {
                builder.Append(separator);
                builder.Append("OR ");
                requiredOrKeywords--;
            }
        }
        foreach (var range in Ranges)
        {
            builder.AppendJoin(separator, RowId, Between, range.From, And, range.To);
            if (requiredOrKeywords > 0)
            {
                builder.Append(separator);
                builder.Append("OR ");
                requiredOrKeywords--;
            }
        }
        return builder.ToString();
    }
}

public static class ExpressionCollectionExtensions
{
    public static string ToQueryString(this IEnumerable<Expression> queryFilters)
    {
        if (queryFilters.Count() > 1 || queryFilters.Count() == 0) throw new Exception("Can't accept more than 1 query filter");

        var filter = queryFilters.First();
        if (filter is IdFilterExpression idFilter)
            return idFilter.ToQueryString();
        return "";
    }
}
