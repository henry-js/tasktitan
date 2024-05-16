// namespace TaskTitan.Lib;

// public record Result<T>
// {
//     private Result(bool success, T? value)
//     {
//         IsSuccess = success;
//         Value = value;
//     }

//     public Result(bool success, T? value, string[] errors)
//     {

//     }

//     public bool IsSuccess { get; }
//     public T? Value { get; }

//     public static Result<T> Success(T value)
//     {
//         var result = new Result<T>(true, value);
//         return result;
//     }
//     public static Result<T> Fail(T value, params string[] errors)
//     {
//         var result = new Result<T>(false, value, errors);
//         return result;
//     }
// }
