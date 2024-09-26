using System.Net;

namespace Documents.Client;

public class OperationResult<T>(bool isSuccess, T data = default!, HttpStatusCode? statusCode = null, string errorMessage = "")
{
    public static OperationResult<T> Success(T data)
    {
        return new OperationResult<T>(true, data);
    }

    public static OperationResult<T> Failure(string errorMessage, HttpStatusCode? statusCode = null)
    {
        return new OperationResult<T>(false, default!, statusCode, errorMessage);
    }

    public bool IsSuccess { get; set; } = isSuccess;
    public T Data { get; set; } = data;
    public HttpStatusCode? StatusCode { get; set; } = statusCode;
    public string ErrorMessage { get; set; } = errorMessage;
}
