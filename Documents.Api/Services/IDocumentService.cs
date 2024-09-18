using Documents.Api.Dto;
using System.Reflection.Metadata;

namespace Documents.Api.Services;

public interface IDocumentService
{
    Task<List<Document>> GetDocumentsByCustomerIdAsync(string customerId, string documentType = null);
    Task<List<Document>> GetDocumentsByCustomerAndOrderAsync(string customerId, string orderCode, string documentType = null);
    Task<Document> StoreDocumentAsync(DocumentDto documentDto);
}
