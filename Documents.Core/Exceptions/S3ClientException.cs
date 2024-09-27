namespace Documents.Core.Exceptions;

public class S3ClientException(string message, Exception innerException) : Exception(message, innerException)
{
}