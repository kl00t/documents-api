using Documents.Service.Models;

namespace Documents.Api.Dto;

public record StoreDocumentRequest(
    string CustomerId, 
    string DocumentType)
{
    public Document ToDomain()
    {
        return new Document
        {
            CustomerId = CustomerId,
            DocumentType = DocumentType
        };
    }
}