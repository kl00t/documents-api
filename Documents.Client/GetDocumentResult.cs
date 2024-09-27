namespace Documents.Client;

public class GetDocumentResult(string fileName, string documentType, string key, string contentType, string customerId)
{
    public string FileName { get; private set; } = fileName;

    public string DocumentType { get; private set; } = documentType;

    public string DocumentId { get; private set; } = key;

    public string ContentType { get; private set; } = contentType;

    public string CustomerId { get; private set; } = customerId;
}