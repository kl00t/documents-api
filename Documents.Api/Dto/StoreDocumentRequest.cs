using Documents.Service.Models;

namespace Documents.Api.Dto;

public record StoreDocumentRequest(
    string CustomerId,
    string OrderCode,
    string DocumentType)
{
    public Document ToDomain()
    {
        return new Document
        {
            CustomerId = CustomerId.ToLower(),
            OrderCode = OrderCode.ToLower(),
            DocumentType = DocumentType.ToLower(),
        };
    }
}