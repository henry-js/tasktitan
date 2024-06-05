using TaskTitan.Core.Queries;
using TaskTitan.Lib.Queries;

namespace TaskTitan.Lib.Expressions;

public abstract record Expression { }
public record TagFilter(char Sign, string Value) : Expression;
public record AttributeFilter(string attribute, string Value) : Expression;
public record GroupedExpression(Expression Left, string Operator, Expression Right) : Expression;
public record IdFilterExpression(IdRange[] Ranges, SoleIds Ids) : Expression;
