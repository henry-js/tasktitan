using TaskTitan.Data.Reports;

namespace TaskTitan.Core;

public abstract record Expr;
public record BinaryFilter(Expr Left, BinaryOperator Operator, Expr Right) : Expr;
public enum BinaryOperator { And, Or }
public record CommandExpression(IEnumerable<TaskAttribute> Properties, string Input) : Expr;
public record FilterExpression(Expr Expr) : Expr;
public record ReportExpression(ReportDefinition Report) : Expr;
