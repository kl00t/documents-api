using Documents.Service.Models;

namespace Documents.Service;

public interface IDocumentService
{
    Task<Result<Document>> StoreDocumentAsync(Stream fileStream, Models.Document document);
    Task<Result<Document>> GetDocumentUrlAsync(Document document);
    Task<Result<bool>> DeleteDocumentAsync(Document document);
}