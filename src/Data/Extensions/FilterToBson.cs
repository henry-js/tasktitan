using System.Runtime.CompilerServices;

using LiteDB;

using TaskTitan.Core;
using TaskTitan.Core.Enums;
using TaskTitan.Data.Parsers;

using static TaskTitan.Core.Tag;

namespace TaskTitan.Data.Extensions;

public static class FilterToBson
{
    static DateParser? _dateParser;
    public static BsonExpression ToBsonExpression(this FilterExpression filter, DateParser dateParser)
    {
        _dateParser = dateParser;
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
        else if (attr is Tag tag)
        {
            return ParseTag(tag);
        }

        throw new Exception($"Unsupported property type {attr.GetType()}");

        BsonExpression ParseDateTimeAttribute(TaskAttribute<DateTime> attribute)
        {
            return attribute.Modifier switch
            {
                ColModifier.NoModifier => Query.EQ(attribute.Name, attribute.Value.Date),
                ColModifier.Equals or ColModifier.Is => Query.EQ(attribute.Name, attribute.Value),
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
                ColModifier.NoModifier => Query.EQ(attribute.Name, attribute.Value),
                ColModifier.Equals or ColModifier.Is => Query.EQ(attribute.Name, attribute.Value),
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
                ColModifier.NoModifier => Query.EQ(attribute.Name, attribute.Value),
                ColModifier.Equals or ColModifier.Is => Query.EQ(attribute.Name, attribute.Value),
                ColModifier.Below => Query.LT(attribute.Name, attribute.Value),
                ColModifier.Above => Query.GTE(attribute.Name, attribute.Value),
                ColModifier.Not or ColModifier.Isnt => Query.Not(attribute.Name, attribute.Value),
                _ => throw new SwitchExpressionException($"Modifier {attribute.Modifier} is not supported for Text attributes"),
            };
        }
        BsonExpression ParseTag(Tag tag)
        {
            return tag switch
            {
                { IsSynthetic: true } => Synthetic(tag),
                _ => throw new SwitchExpressionException($"Tag not supported, \n TaskTag: {tag?.ToString() ?? "NULL"}")
            };

            BsonExpression Synthetic(Tag tag)
            {
                return tag switch
                {
                    { Name: nameof(SyntheticTag.Waiting), Modifier: ColModifier.Include } => Query.Not(nameof(TaskItem.Wait), null),
                    { Name: nameof(SyntheticTag.Waiting), Modifier: ColModifier.Exclude } => Query.EQ(nameof(TaskItem.Wait), null),
                    { Name: nameof(SyntheticTag.DueToday), Modifier: ColModifier.Include } => Query.EQ(nameof(TaskItem.Wait), DateTime.UtcNow),
                    { Name: nameof(SyntheticTag.Today), Modifier: ColModifier.Include } => Query.EQ(nameof(TaskItem.Wait), DateTime.UtcNow),
                    { Name: nameof(SyntheticTag.Overdue), Modifier: ColModifier.Include } => Query.LT(nameof(TaskItem.Due), DateTime.UtcNow),
                    { Name: nameof(SyntheticTag.Week), Modifier: ColModifier.Include } => Query.Between(nameof(TaskItem.Due), _dateParser?.Parse("monday"), _dateParser?.Parse("sunday")),
                    { Name: nameof(SyntheticTag.Month), Modifier: ColModifier.Include } => Query.Between(nameof(TaskItem.Due), _dateParser?.Parse("som"), _dateParser?.Parse("eom")),
                    { Name: nameof(SyntheticTag.Quarter), Modifier: ColModifier.Include } => Query.Between(nameof(TaskItem.Due), _dateParser?.Parse("som"), _dateParser?.Parse("eom")),
                    { Name: nameof(SyntheticTag.Year), Modifier: ColModifier.Include } => Query.Between(nameof(TaskItem.Due), _dateParser?.Parse("som"), _dateParser?.Parse("eom")),
                    { Name: nameof(SyntheticTag.Active), Modifier: ColModifier.Include } => Query.Not(nameof(TaskItem.Start), null),
                    { Name: nameof(SyntheticTag.Active), Modifier: ColModifier.Exclude } => Query.EQ(nameof(TaskItem.Start), null),
                    { Name: nameof(SyntheticTag.Scheduled), Modifier: ColModifier.Include } => Query.Not(nameof(TaskItem.Scheduled), null),
                    { Name: nameof(SyntheticTag.Scheduled), Modifier: ColModifier.Exclude } => Query.EQ(nameof(TaskItem.Scheduled), null),
                    { Name: nameof(SyntheticTag.Until), Modifier: ColModifier.Include } => Query.Not(nameof(TaskItem.Until), null),
                    { Name: nameof(SyntheticTag.Until), Modifier: ColModifier.Exclude } => Query.EQ(nameof(TaskItem.Until), null),
                    { Name: nameof(SyntheticTag.Pending), Modifier: ColModifier.Include } => Query.EQ(nameof(TaskItem.Status), nameof(TaskItemStatus.Pending)),
                    { Name: nameof(SyntheticTag.Completed), Modifier: ColModifier.Include } => Query.EQ(nameof(TaskItem.Status), nameof(TaskItemStatus.Completed)),
                    { Name: nameof(SyntheticTag.Deleted), Modifier: ColModifier.Include } => Query.EQ(nameof(TaskItem.Status), nameof(TaskItemStatus.Deleted)),
                    _ => throw new SwitchExpressionException($"Tag not supported, \n TaskTag: {tag?.ToString() ?? "NULL"}")
                };
            }
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
