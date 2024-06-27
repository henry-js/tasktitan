using System.Text;

namespace TaskTitan.Core.Expressions;

public record TagFilterExpression(char Sign, string Value) : Expression
{
    // public override string ToQueryString(IExpressionConversionOptions? options = null)
    // {
    //     var builder = new StringBuilder();

    //     builder.AppendLine("-- tag filter placeholder");
    //     return builder.ToString();
    // }
}
