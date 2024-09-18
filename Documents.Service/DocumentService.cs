namespace Documents.Service;

public class DocumentService : IDocumentService
{
    public DocumentService()
    {
        
    }

    public async Task<List<Models.Document>> GetDocumentsByCustomerAndOrderAsync(string customerId, string orderCode, string documentType = null)
    {
        return [new Models.Document { CustomerId = customerId, OrderCode = orderCode }];
    }

    public async Task<List<Models.Document>> GetDocumentsByCustomerIdAsync(string customerId, string documentType = null)
    {
        return [new Models.Document { CustomerId = customerId }];
    }

    public async Task<Models.Document> StoreDocumentAsync(Models.Document document)
    {
        return document;
    }
}