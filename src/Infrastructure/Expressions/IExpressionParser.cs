namespace TaskTitan.Infrastructure.Expressions;

public interface IExpressionParser
{
    Expression ParseFilter(string expression);
}
