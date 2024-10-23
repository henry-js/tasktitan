using TaskTitan.Data.Reports;

namespace TaskTitan.Data.Expressions;

public abstract record Expr;
public record BinaryFilter(Expr Left, BinaryOperator Operator, Expr Right) : Expr;
public enum BinaryOperator { And, Or }
public record CommandExpression(IEnumerable<TaskProperty> Properties, string Input) : Expr;
public record FilterExpression(Expr Expr) : Expr;
public record ReportExpression(CustomReport Report) : Expr;
