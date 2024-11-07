using TaskTitan.Data.Reports;

namespace TaskTitan.Core;

public abstract record Expr;
public record BinaryFilter(Expr Left, BinaryOperator Operator, Expr Right) : Expr;
public enum BinaryOperator { And, Or }
public record CommandExpression : Expr

{
    public CommandExpression(IEnumerable<TaskAttribute> properties, string input)
    {
        Properties = properties;
        Input = input;
    }
    public CommandExpression(Dictionary<string, TaskAttribute> dict, string input)
    {
        Dict = dict;
        Input = input;
    }

    public IEnumerable<TaskAttribute> Properties { get; } = [];
    public string Input { get; }
    public Dictionary<string, TaskAttribute> Dict { get; } = [];
}

public record FilterExpression(Expr Expr) : Expr;
public record ReportExpression(ReportDefinition Report) : Expr;
