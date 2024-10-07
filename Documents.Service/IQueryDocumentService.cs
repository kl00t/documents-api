using Documents.Service.Models;

namespace Documents.Service;

public interface IQueryDocumentService
{
    Task<Result<Document>> GetDocumentUrlAsync(Document document);
    Task<Result<List<Document>>> GetAllDocumentsAsync(Document document);
}