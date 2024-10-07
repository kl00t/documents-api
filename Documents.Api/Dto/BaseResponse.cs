namespace Documents.Api.Dto;

public class BaseResponse<T>
{
    public bool IsSuccess { get; }
    public T Data { get; }
    public string? ErrorMessage { get; }

    // Private constructor to ensure Result is created via Success or Failure static methods.
    private BaseResponse(bool isSuccess, T data, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data!;
        ErrorMessage = errorMessage;
    }

    // Factory method for successful result
    public static BaseResponse<T> Success(T data)
    {
        return new BaseResponse<T>(true, data, null);
    }

    // Factory method for failure result
    public static BaseResponse<T> Failure(string errorMessage)
    {
        return new BaseResponse<T>(false, default!, errorMessage);
    }

    // Implicit operator to check success state directly
    public static implicit operator bool(BaseResponse<T> result)
    {
        return result.IsSuccess;
    }
}
