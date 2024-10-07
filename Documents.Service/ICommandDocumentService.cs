using Documents.Service.Models;

namespace Documents.Service;

public interface ICommandDocumentService
{
    Task<Result<Document>> StoreDocumentAsync(Stream fileStream, Models.Document document);
    Task<Result<bool>> DeleteDocumentAsync(Document document);
}
