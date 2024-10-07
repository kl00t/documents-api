namespace Documents.Service;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Data { get; }
    public string? ErrorMessage { get; }

    // Private constructor to ensure Result is created via Success or Failure static methods.
    private Result(bool isSuccess, T data, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data!;
        ErrorMessage = errorMessage;
    }

    // Factory method for successful result
    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, null);
    }

    // Factory method for failure result
    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(false, default!, errorMessage);
    }

    // Implicit operator to check success state directly
    public static implicit operator bool(Result<T> result)
    {
        return result.IsSuccess;
    }
}
