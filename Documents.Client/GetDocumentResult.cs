namespace Documents.Client;

public class GetDocumentResult
{
    public GetDocumentResult(Stream responseStream, string contentType, string fileName)
    {
        ResponseStream = responseStream;
        ContentType = contentType;
        FileName = fileName;
    }

    public Stream ResponseStream { get; private set; }
    public string ContentType { get; private set; }
    public string FileName { get; private set; }
}