using Documents.Service.Models;

namespace Documents.Api.Dto;

public record StoreDocumentRequest(
    string CustomerId,
    string DocumentId,
    string FileName,
    string ContentType,
    string DocumentType)
{
    public Document ToDomain()
    {
        return new Document
        {
            CustomerId = CustomerId,
            DocumentId = DocumentId,
            FileName = FileName,
            ContentType = ContentType,
            DocumentType = DocumentType
        };
    }
}