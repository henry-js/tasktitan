using System.Runtime.CompilerServices;

using LiteDB;

using TaskTitan.Data.Expressions;

using static TaskTitan.Data.Enums;

namespace TaskTitan.Data.Extensions;

public static class FilterToBson
{
    public static BsonExpression ToBsonExpression(this FilterExpression filter)
    {
        return ExprToBsonExpression(filter.Expr);
    }

    private static BsonExpression ExprToBsonExpression(Expr expr, int depth = 0)
    {
        return expr switch
        {
            BinaryFilter bf => BinaryFilterToBsonExpression(bf, depth),
            TaskAttribute attr => AttributeToBsonExpression(attr),
            _ => throw new SwitchExpressionException()
        };
    }

    private static BsonExpression TagToBsonExpression(TaskTag tag)
    {
        throw new NotImplementedException();
    }

    private static BsonExpression AttributeToBsonExpression(TaskAttribute attr)
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
        else if (attr is TaskTag tag)
        {
            return ParseTag(tag);
        }

        throw new Exception($"Unsupported property type {attr.GetType()}");

        BsonExpression ParseDateTimeAttribute(TaskAttribute<DateTime> attribute)
        {
            return attribute.Modifier switch
            {
                ColModifier.Equals or ColModifier.Is or null => Query.EQ(attribute.Name, attribute.Value),
                ColModifier.Before => Query.LT(attribute.Name, attribute.Value),
                ColModifier.After => Query.GTE(attribute.Name, attribute.Value),
                ColModifier.Not => Query.Not(attribute.Name, attribute.Value),
                _ => throw new SwitchExpressionException($"Modifier {attribute.Modifier} is not supported for Date attributes"),
            };
        }
        BsonExpression ParseTextAttribute(TaskAttribute<string> attribute)
        {
            return attribute.Modifier switch
            {
                ColModifier.Equals or ColModifier.Is or null => Query.EQ(attribute.Name, attribute.Value),
                ColModifier.Isnt => Query.Not(attribute.Name, attribute.Value),
                ColModifier.Has or ColModifier.Contains => Query.Contains(attribute.Name, attribute.Value),
                ColModifier.Hasnt => Query.Not(attribute.Name, attribute.Value),
                ColModifier.Startswith => Query.StartsWith(attribute.Name, attribute.Value),
                ColModifier.Endswith => BsonExpression.Create($"{attribute.Name} LIKE {new BsonValue("%" + attribute.Value)}"),
                _ => throw new SwitchExpressionException($"Modifier {attribute.Modifier} is not supported for Text attributes"),
            };
        }
        BsonExpression ParseNumberAttribute(TaskAttribute<double> attribute)
        {
            return attribute.Modifier switch
            {
                ColModifier.Equals or ColModifier.Is or null => Query.EQ(attribute.Name, attribute.Value),
                ColModifier.Below => Query.LT(attribute.Name, attribute.Value),
                ColModifier.Above => Query.GTE(attribute.Name, attribute.Value),
                ColModifier.Not or ColModifier.Isnt => Query.Not(attribute.Name, attribute.Value),
                _ => throw new SwitchExpressionException($"Modifier {attribute.Modifier} is not supported for Text attributes"),
            };
        }
        BsonExpression ParseTag(TaskTag tag)
        {
            return tag switch
            {
                { Name: "WAITING", Modifier: ColModifier.Exclude } => Query.EQ(nameof(TaskItem.Wait), null),
                { Name: "WAITING", Modifier: ColModifier.Include } => Query.Not(nameof(TaskItem.Wait), null),
                _ => throw new SwitchExpressionException($"Tag not supported, \n TaskTag: {tag?.ToString() ?? "NULL"}")
            };
        }
    }


    private static BsonExpression BinaryFilterToBsonExpression(BinaryFilter bf, int depth)
    {
        depth++;
        var left = ExprToBsonExpression(bf.Left, depth);

        var right = ExprToBsonExpression(bf.Right, depth);

        var expression = bf.Operator switch
        {
            BinaryOperator.And => Query.And(left, right),
            BinaryOperator.Or => Query.Or(left, right),
            _ => throw new Exception()
        };
        depth--;
        // return depth == 0 ? $"{left} {Operator} {right}" : $"({left} {Operator} {right})";
        return expression;
    }
}
