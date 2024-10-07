using Documents.Client;
using Documents.Client.Constants;
using Documents.Service.Models;
using Microsoft.Extensions.Logging;

namespace Documents.Service;

public class QueryDocumentService(ILogger<QueryDocumentService> logger, IDocumentStorageClient documentStorageClient) : IQueryDocumentService
{
    private static string ConstructKey(string customerId, string orderCode, string documentId)
    {
        ArgumentNullException.ThrowIfNull(customerId);
        ArgumentNullException.ThrowIfNull(orderCode);
        ArgumentNullException.ThrowIfNull(documentId);
        return $"{customerId}/{orderCode}/{documentId}";
    }

    public async Task<Result<Document>> GetDocumentUrlAsync(string customerId, string orderCode, string documentId)
    {
        try
        {
            string key = ConstructKey(customerId, orderCode, documentId);
            var response = await documentStorageClient.GetObjectMetaData(key);

            if (!response.IsSuccess)
            {
                logger.LogError("Failed to get object metadata {DocumentId} for customer {CustomerId}.", documentId, customerId);
                return Result<Document>.Failure($"Failed to generate pre-signed URL document {documentId} for customer {customerId}.");
            }

            var preSignedUrlresponse = await documentStorageClient.GetPreSignedUrlAsync(key);
            if (!preSignedUrlresponse.IsSuccess)
            {
                logger.LogError("Failed to generate pre-signed URL for document {DocumentId} for customer {CustomerId}.", documentId, customerId);
                return Result<Document>.Failure($"Failed to generate pre-signed URL document {documentId} for customer {customerId}.");
            }

            return Result<Document>.Success(new Document
            {
                CustomerId = customerId,
                OrderCode = orderCode,
                DocumentId = documentId,
                FileName = documentStorageClient.GetMetadataValue(response.Data, MetadataConstants.FileName),
                DocumentType = documentStorageClient.GetMetadataValue(response.Data, MetadataConstants.DocumentType),
                ContentType = documentStorageClient.GetMetadataValue(response.Data, MetadataConstants.ContentType),
                Url = preSignedUrlresponse.Data,
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get document {DocumentId} for customer {CustomerId}.", documentId, customerId);
            return Result<Document>.Failure($"Failed to get document {documentId} for customer {customerId}.");
        }
    }

    public async Task<Result<List<Document>>> GetAllDocumentsAsync(string customerId, string orderCode, string documentType)
    {
        try
        {
            var response = await documentStorageClient.GetObjectsAsync(customerId, documentType);
            if (!response.IsSuccess)
            {
                logger.LogError("Failed to get objects for customer {CustomerId}.", customerId);
                return Result<List<Document>>.Failure($"Failed to get objects for customer {customerId}.");
            }

            var documents = (from d in response.Data
                             let ddd = new Document
                             {
                                 CustomerId = d.CustomerId,
                                 OrderCode = d.OrderCode,
                                 DocumentId = d.DocumentId,
                                 FileName = d.FileName,
                                 DocumentType = d.DocumentType,
                                 ContentType = d.ContentType
                             }
                             select ddd).ToList();

            return Result<List<Document>>.Success(documents);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get documents for customer {CustomerId}.", customerId);
            return Result<List<Document>>.Failure($"Failed to get documents for customer {customerId}.");
        }
    }
}