using Documents.Service.Models;

namespace Documents.Api.Dto;

public record DocumentResponse(
    string CustomerId,
    string DocumentId,
    MetaData MetaData)
{
    public static DocumentResponse FromDomain(Document document)
    {
        return new DocumentResponse(
            document.CustomerId ?? string.Empty,
            document.DocumentId ?? string.Empty,
            new MetaData(
                document.FileName ?? string.Empty, 
                document.ContentType ?? string.Empty, 
                document.DocumentType ?? string.Empty));
    }

    public static IEnumerable<DocumentResponse> FromDomain(IEnumerable<Document> documents)
    {
        return (from document in documents
                    select FromDomain(document)).ToList();
    }
}

public record MetaData(string FileName, string contentType, string DocumentType)
{
}