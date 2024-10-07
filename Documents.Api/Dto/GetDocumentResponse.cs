using Documents.Service.Models;

namespace Documents.Api.Dto;

public record GetDocumentResponse(string CustomerId, string OrderCode, string DocumentId, string Url)
{
    public static GetDocumentResponse FromDomain(Document document)
    {
        return new GetDocumentResponse(
            document.CustomerId ?? string.Empty,
            document.OrderCode ?? string.Empty,
            document.DocumentId ?? string.Empty,
            document.Url ?? string.Empty);
    }
}
