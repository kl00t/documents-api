using Documents.Service.Models;

namespace Documents.Api.Dto;

public record DocumentResponse(string FileName)
{
    public static DocumentResponse FromDomain(Document document)
    {
        return new DocumentResponse(document.FileName!);
    }

    public static IEnumerable<DocumentResponse> FromDomain(IEnumerable<Document> documents)
    {
        return (from document in documents
                    select FromDomain(document)).ToList();
    }
}