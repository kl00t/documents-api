using Documents.Service.Models;

namespace Documents.Api.Dto;

public record DeleteDocumentRequest(string CustomerId, string OrderCode, string DocumentId)
{
    public Document ToDomain()
    {
        return new Document
        {
            CustomerId = CustomerId,
            OrderCode = OrderCode,
            DocumentId = DocumentId
        };
    }
}