using System.Text;

using TaskTitan.Core.Queries;

using static TaskTitan.Data.DbConstants.KeyWords;
using static TaskTitan.Data.DbConstants.TasksTable;

namespace TaskTitan.Lib.Expressions;

public record IdFilterExpression(IdRange[] Ranges, SoleIds Ids) : Expression
{
    public override string ToQueryString(IExpressionConversionOptions? options = null)
    {
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
