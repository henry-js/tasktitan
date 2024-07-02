namespace TaskTitan.Core.OperationResults;

public record Result
{
    internal Result(bool success, Error error)
    {
        if ((success && error != Error.None) || (!success && error == Error.None))
            throw new ArgumentException("Invalid error", nameof(error));
        IsSuccess = success;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

public sealed record Error(int Code, string Description)
{
    public static readonly Error None = new(0, string.Empty);
}

public record Result<T> : Result
{
    protected internal Result(bool success, Error error, T value) : base(success, error)
    {
        Value = value;
    }

    protected internal Result(bool succes, Error error) : base(succes, error)
    {
        Value = default;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new Result<T>(true, Error.None, value);
    new public static Result<T> Failure(Error error) => new Result<T>(false, error);
}
