namespace Documents.Core.Exceptions;

public class ServiceException(string message, Exception innerException) : Exception(message, innerException)
{
}