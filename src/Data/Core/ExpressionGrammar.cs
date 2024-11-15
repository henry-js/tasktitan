
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Reports;

namespace TaskTitan.Core;

public abstract record Expr;
public record BinaryFilter(Expr Left, BinaryOperator Operator, Expr Right) : Expr;
public enum BinaryOperator { And, Or }
public record CommandExpression : Expr

{
    // public CommandExpression(IEnumerable<TaskAttribute> properties, string input)
    // {
    //     Properties = properties.ToDictionary(x => x.Name);
    //     Input = input;
    // }
    public CommandExpression(Dictionary<string, TaskAttribute> dict, string input)
    {
        Properties = dict;
        Input = input;
    }

    public string Input { get; }
    public Dictionary<string, TaskAttribute> Properties { get; } = [];

    public void AddModification<T>(string field, T value)
    {
        if (!TaskColumns.ColumnTypes.ContainsKey(field)) throw new Exception($"Invalid column {field}");

        Properties.TryAdd(field, TaskAttributeFactory.CreateBuiltIn(field, value));
    }
}

public record FilterExpression(Expr Expr) : Expr;
public record ReportExpression(ReportDefinition Report) : Expr;
