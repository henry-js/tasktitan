namespace TaskTitan.Lib.Expressions;

public interface IExpressionParser
{
    Expression ParseFilter(string expression);
}
