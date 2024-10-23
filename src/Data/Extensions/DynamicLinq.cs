using System.Runtime.CompilerServices;

using TaskTitan.Data.Expressions;

using static TaskTitan.Data.Enums;

namespace TaskTitan.Data.Extensions;
public static class DynamicLinq
{
    public static string ToDynamicLinq(this FilterExpression filter)
    {
        return ExprToLinq(filter.Expr);
    }

    private static string ExprToLinq(Expr expr, int depth = 0)
    {
        return expr switch
        {
            BinaryFilter bf => BinaryFilterToLinq(bf, depth),
            TaskProperty attr => AttributeToLinq(attr),
            _ => throw new SwitchExpressionException($"Cannot convert expression: {expr.GetType()} to linq")
        };
    }

    private static string AttributeToLinq(TaskProperty attr)
    {
        if (attr is TaskProperty<DateTime> t)
        {
            return ParseDateTimeAttribute(t);
        }
        else if (attr is TaskProperty<double> d)
        {
            return ParseNumberAttribute(d);
        }
        else if (attr is TaskProperty<string> s)
        {
            return ParseTextAttribute(s);
        }

        throw new Exception($"Unsupported property type {attr.GetType()}");

        string ParseDateTimeAttribute(TaskProperty<DateTime> attribute)
        {
            return attribute.Modifier switch
            {
                ColModifier.Equals or null => $"{attribute.PropertyName} == {attribute.Value}",
                ColModifier.Before => $"{attribute.PropertyName} < {attribute.Value}",
                ColModifier.After => $"{attribute.PropertyName} >= {attribute.Value}",
                ColModifier.Is => $"{attribute.PropertyName} == {attribute.Value}",
                ColModifier.Not => $"{attribute.PropertyName} != {attribute.Value}",
                _ => throw new SwitchExpressionException($"Modifier {attribute.Modifier} is not supported for Date attributes"),
            };
        }
        string ParseTextAttribute(TaskProperty<string> attribute)
        {
            return attribute.Modifier switch
            {
                ColModifier.Equals or null => $"{attribute.PropertyName}.Equals(\"{attribute.Value}\", StringComparison.CurrentCultureIgnoreCase)",
                ColModifier.Isnt => $"!{attribute.PropertyName}.Equals(\"{attribute.Value}\", StringComparison.CurrentCultureIgnoreCase)",
                ColModifier.Has or ColModifier.Contains => $"{attribute.PropertyName}.Contains(\"{attribute.Value}\", StringComparison.CurrentCultureIgnoreCase)",
                ColModifier.Hasnt => $"!{attribute.PropertyName}.Contains(\"{attribute.Value}\", StringComparison.CurrentCultureIgnoreCase)",
                ColModifier.Startswith => $"{attribute.PropertyName}.StartsWith(\"{attribute.Value}\", StringComparison.CurrentCultureIgnoreCase)",
                ColModifier.Endswith => $"{attribute.PropertyName}.EndsWith(\"{attribute.Value}\", StringComparison.CurrentCultureIgnoreCase)",
                _ => throw new SwitchExpressionException($"Modifier {attribute.Modifier} is not supported for Text attributes"),
            };
        }
        string ParseNumberAttribute(TaskProperty<double> attribute)
        {
            return attribute.Modifier switch
            {
                ColModifier.Equals or null => $"{attribute.PropertyName} == {attribute.Value}",
                ColModifier.Below => $"{attribute.PropertyName} < {attribute.Value}",
                ColModifier.Above => $"{attribute.PropertyName} >= {attribute.Value}",
                ColModifier.Isnt => $"{attribute.PropertyName} != {attribute.Value}",
                _ => throw new SwitchExpressionException($"Modifier {attribute.Modifier} is not supported for Text attributes"),
            };
        }
    }

    private static string BinaryFilterToLinq(BinaryFilter bf, int depth = 0)
    {
        depth++;
        var left = ExprToLinq(bf.Left, depth);

        var right = ExprToLinq(bf.Right, depth);

        var Operator = bf.Operator switch
        {
            BinaryOperator.And => "&&",
            BinaryOperator.Or => "||",
            _ => throw new Exception()
        };
        depth--;
        return depth == 0 ? $"{left} {Operator} {right}" : $"({left} {Operator} {right})";
    }
}
