namespace TaskTitan.Lib.Expressions;

public interface IExpressionParser
{
    object ParseFilter(string expression);
}
