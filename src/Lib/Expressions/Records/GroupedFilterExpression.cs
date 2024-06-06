using System.Text;

using TaskTitan.Lib.Dates;

namespace TaskTitan.Lib.Expressions;

public record GroupedFilterExpression(Expression Left, string Operator, Expression Right) : Expression
{
    public override string ToQueryString(IExpressionConversionOptions? options = null)
    {
        options ??= AttributeFilterConversionOptions.Default;
        var builder = new StringBuilder();
        var left = Left.ToQueryString(options);
        builder.Append('(').Append(left).Append(')');

        builder.Append(' ').Append(this.Operator.ToUpper()).Append(' ');

        var right = Right.ToQueryString(options);
        builder.Append('(').Append(right).Append(')');

        return builder.ToString();
    }
}
