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
            TaskAttribute attr => AttributeToLinq(attr),
            _ => throw new SwitchExpressionException($"Cannot convert expression: {expr.GetType()} to linq")
        };
    }

    private static string AttributeToLinq(TaskAttribute attr)
    {
        if (attr is TaskAttribute<DateTime> t)
        {
            return ParseDateTimeAttribute(t);
        }
        else if (attr is TaskAttribute<double> d)
        {
            return ParseNumberAttribute(d);
        }
        else if (attr is TaskAttribute<string> s)
        {
            return ParseTextAttribute(s);
        }

        throw new Exception($"Unsupported property type {attr.GetType()}");

        string ParseDateTimeAttribute(TaskAttribute<DateTime> attribute)
        {
            return attribute.Modifier switch
            {
                ColModifier.Equals or null => $"{attribute.Name} == {attribute.Value}",
                ColModifier.Before => $"{attribute.Name} < {attribute.Value}",
                ColModifier.After => $"{attribute.Name} >= {attribute.Value}",
                ColModifier.Is => $"{attribute.Name} == {attribute.Value}",
                ColModifier.Not => $"{attribute.Name} != {attribute.Value}",
                _ => throw new SwitchExpressionException($"Modifier {attribute.Modifier} is not supported for Date attributes"),
            };
        }
        string ParseTextAttribute(TaskAttribute<string> attribute)
        {
            return attribute.Modifier switch
            {
                ColModifier.Equals or null => $"{attribute.Name}.Equals(\"{attribute.Value}\", StringComparison.CurrentCultureIgnoreCase)",
                ColModifier.Isnt => $"!{attribute.Name}.Equals(\"{attribute.Value}\", StringComparison.CurrentCultureIgnoreCase)",
                ColModifier.Has or ColModifier.Contains => $"{attribute.Name}.Contains(\"{attribute.Value}\", StringComparison.CurrentCultureIgnoreCase)",
                ColModifier.Hasnt => $"!{attribute.Name}.Contains(\"{attribute.Value}\", StringComparison.CurrentCultureIgnoreCase)",
                ColModifier.Startswith => $"{attribute.Name}.StartsWith(\"{attribute.Value}\", StringComparison.CurrentCultureIgnoreCase)",
                ColModifier.Endswith => $"{attribute.Name}.EndsWith(\"{attribute.Value}\", StringComparison.CurrentCultureIgnoreCase)",
                _ => throw new SwitchExpressionException($"Modifier {attribute.Modifier} is not supported for Text attributes"),
            };
        }
        string ParseNumberAttribute(TaskAttribute<double> attribute)
        {
            return attribute.Modifier switch
            {
                ColModifier.Equals or null => $"{attribute.Name} == {attribute.Value}",
                ColModifier.Below => $"{attribute.Name} < {attribute.Value}",
                ColModifier.Above => $"{attribute.Name} >= {attribute.Value}",
                ColModifier.Isnt => $"{attribute.Name} != {attribute.Value}",
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
