using Documents.Service.Models;

namespace Documents.Api.Dto;

public record GetDocumentResponse(string CustomerId, string DocumentId, string Url)
{
    public static GetDocumentResponse FromDomain(Document document)
    {
        return new GetDocumentResponse(
            document.CustomerId ?? string.Empty,
            document.DocumentId ?? string.Empty,
            document.Url ?? string.Empty);
    }
}
