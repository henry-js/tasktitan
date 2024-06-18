using System.Text;

namespace TaskTitan.Core.Expressions;

public record AttributeFilterExpression(string attribute, string Value) : Expression
{
    public override string ToQueryString(IExpressionConversionOptions? options = null)
    {
        // options ??= AttributeFilterConversionOptions.Default;
        var builder = new StringBuilder();
        if (options.StandardDateAttributes.Contains(attribute))
        {
            DateTime date = options.StandardDateConverter.ConvertFrom(Value) ?? throw new Exception($"Unsupported date format: {Value}");
            builder.Append($"date({attribute})").Append(" = ").Append($"date('{date:yyyy-MM-dd}')");
            return builder.ToString();
        }
        if (options.StandardStringAttributes.Contains(attribute))
        {
            builder.Append(attribute).Append(" = ").Append($"'{Value}'");
            return builder.ToString();
        }
        return builder.ToString();
    }
}
