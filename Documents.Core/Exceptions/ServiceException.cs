namespace Documents.Service.Exceptions;

public class ServiceException(string message, Exception innerException) : Exception(message, innerException)
{
}