namespace TaskTitan.Core.OperationResults;

public class Result
{
    private Result(bool success, Error error)
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
