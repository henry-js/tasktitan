namespace TaskTitan.Lib.Expressions;

public abstract record Expression { }
public record TagFilter(char Sign, string Value) : Expression;
public record AttributeFilter(string attribute, string Value) : Expression;
public record GroupedExpression(Expression Left, string Operator, Expression Right) : Expression;
