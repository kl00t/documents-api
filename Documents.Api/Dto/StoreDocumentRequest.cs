using Documents.Service.Models;

namespace Documents.Api.Dto;

public record StoreDocumentRequest(
    string FileName,
    string ContentType,
    string ContentBody)
{
    public Document ToDomain()
    {
        return new Document
        {
            FileName = FileName,
            ContentType = ContentType,
            ContentBody = ContentBody
        };
    }
}
