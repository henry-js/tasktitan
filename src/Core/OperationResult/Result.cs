// namespace TaskTitan.Core.OperationResults;

// public abstract record Result
// {
//     public bool IsSuccess { get; }
//     public Error Error { get; }

//     protected Result(bool isSuccess, Error error)
//     {
//         IsSuccess = isSuccess;
//         Error = error;
//     }

//     public static Result Success() => new SuccessResult();
//     public static Result<T> Success<T>(T value) => new Result<T>.SuccessResult(value);
//     public static Result Failure(Error error) => new FailureResult(error);

//     public Result<T> ToGeneric<T>() => IsSuccess ? Result<T>.Success(default!) : Result<T>.Failure(Error);

//     private record SuccessResult : Result
//     {
//         public SuccessResult() : base(true, Error.None) { }
//     }

//     private record FailureResult : Result
//     {
//         public FailureResult(Error error) : base(false, error) { }
//     }
// }

// public record Result<T> : Result
// {
//     private readonly T _value;

//     private Result(bool isSuccess, T value, Error error) : base(isSuccess, error)
//     {
//         _value = value;
//     }

//     public T Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot access Value on a failure result.");

//     public static Result<T> Success(T value) => new SuccessResult(value);
//     public new static Result<T> Failure(Error error) => new Result<T>(false, default, error);

//     public static Result<T> FromResult(Result result) => result.ToGeneric<T>();

//     internal record SuccessResult : Result<T>
//     {
//         public SuccessResult(T value) : base(true, value, Error.None) { }
//     }

//     public static implicit operator Result<T>(Result r)
//     {

//     }
// }

// public sealed record Error(int Code, string Description)
// {
//     public static readonly Error None = new(0, string.Empty);
// }
