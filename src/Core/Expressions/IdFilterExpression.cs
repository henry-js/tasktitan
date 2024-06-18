using System.Text;

using TaskTitan.Core.Queries;

namespace TaskTitan.Core.Expressions;

public record IdFilterExpression(IdRange[] Ranges, SoleIds Ids) : Expression
{
    public override string ToQueryString(IExpressionConversionOptions? options = null)
    {
        const char separator = ' ';
        int requiredOrKeywords = Ranges.Count() + (Ids.Count > 0 ? 1 : 0) - 1;
        var builder = new StringBuilder();
        if (Ids.Count > 0)
        {
            var idString = "(" + Ids + ")";
            builder.Append('(')
                   .AppendJoin(separator, "RowId", "IN", idString)
                   .Append(')');
            if (requiredOrKeywords > 0)
            {
                builder.Append(separator);
                builder.Append("OR ");
                requiredOrKeywords--;
            }
        }
        foreach (var range in Ranges)
        {
            builder.Append('(');
            builder.AppendJoin(separator, "RowId", range.ToString());
            builder.Append(')');
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
