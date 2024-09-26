namespace Documents.Service;

public interface IDocumentService
{
    Task<List<Models.Document>> GetDocumentsByCustomerIdAsync(string customerId, string documentType = null);
    Task<List<Models.Document>> GetDocumentsByCustomerAndOrderAsync(string customerId, string orderCode, string documentType = null);
    Task<Models.Document> StoreDocumentAsync(Models.Document document);
}