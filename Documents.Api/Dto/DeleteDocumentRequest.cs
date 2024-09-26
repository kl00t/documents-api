using Documents.Service.Models;

namespace Documents.Api.Dto;

public record DeleteDocumentRequest(string CustomerId, string DocumentId)
{
    public Document ToDomain()
    {
        return new Document
        {
            CustomerId = CustomerId,
            DocumentId = DocumentId
        };
    }
}
