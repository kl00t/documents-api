using Documents.Service.Models;

namespace Documents.Service;

public interface IQueryDocumentService
{
    Task<Result<Document>> GetDocumentUrlAsync(string customerId, string orderCode, string documentId);

    Task<Result<List<Document>>> GetAllDocumentsAsync(string customerId, string orderCode, string? documentType);
}