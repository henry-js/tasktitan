namespace TaskTitan.Core;

public abstract record Expression
{
    // public abstract string ToQueryString(IExpressionConversionOptions? options = null);
}

public interface IExpressionConversionOptions
{
    string[] StandardDateAttributes { get; }
    string[] StandardStringAttributes { get; }
    IStringFilterConverter<DateTime> StandardDateConverter { get; set; }

}
public interface IStringFilterConverter<T> where T : struct
{
    public T? ConvertFrom(string? value);
}
