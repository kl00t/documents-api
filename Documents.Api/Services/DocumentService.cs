using Documents.Api.Dto;
using System.Reflection.Metadata;

namespace Documents.Api.Services;

public class DocumentService : IDocumentService
{
    private readonly ILogger<DocumentService> _documentService;

    public DocumentService(ILogger<DocumentService> documentService)
    {
        _documentService = documentService;
    }

    public async Task<List<Document>> GetDocumentsByCustomerAndOrderAsync(string customerId, string orderCode, string documentType = null)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Document>> GetDocumentsByCustomerIdAsync(string customerId, string documentType = null)
    {
        throw new NotImplementedException();
    }

    public async Task<Document> StoreDocumentAsync(DocumentDto documentDto)
    {
        throw new NotImplementedException();
    }
}
