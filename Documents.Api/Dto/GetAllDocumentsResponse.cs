using Documents.Service.Models;

namespace Documents.Api.Dto;

public record GetAllDocumentsResponse(
    string CustomerId,
    string DocumentId,
    MetaData MetaData)
{
    public static GetAllDocumentsResponse FromDomain(Document document)
    {
        return new GetAllDocumentsResponse(
            document.CustomerId ?? string.Empty,
            document.DocumentId ?? string.Empty,
            new MetaData(
                document.FileName ?? string.Empty, 
                document.ContentType ?? string.Empty, 
                document.DocumentType ?? string.Empty));
    }

    public static IEnumerable<GetAllDocumentsResponse> FromDomain(IEnumerable<Document> documents)
    {
        return (from document in documents
                    select FromDomain(document)).ToList();
    }
}

public record MetaData(string FileName, string contentType, string DocumentType);