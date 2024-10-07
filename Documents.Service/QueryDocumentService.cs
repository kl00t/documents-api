using Documents.Client;
using Documents.Client.Constants;
using Documents.Service.Models;
using Microsoft.Extensions.Logging;

namespace Documents.Service;

public class QueryDocumentService(ILogger<QueryDocumentService> logger, IDocumentStorageClient documentStorageClient) : IQueryDocumentService
{
    private static string ConstructKey(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);
        return $"{document.CustomerId}/{document.OrderCode}/{document.DocumentId}";
    }

    public async Task<Result<Document>> GetDocumentUrlAsync(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        try
        {
            string key = ConstructKey(document);
            var response = await documentStorageClient.GetObjectMetaData(key);

            if (!response.IsSuccess)
            {
                logger.LogError("Failed to get object metadata {DocumentId} for customer {CustomerId}.", document.DocumentId, document.CustomerId);
                return Result<Document>.Failure($"Failed to generate pre-signed URL document {document.DocumentId} for customer {document.CustomerId}.");
            }

            var preSignedUrlresponse = await documentStorageClient.GetPreSignedUrlAsync(key);
            if (!preSignedUrlresponse.IsSuccess)
            {
                logger.LogError("Failed to generate pre-signed URL for document {DocumentId} for customer {CustomerId}.", document.DocumentId, document.CustomerId);
                return Result<Document>.Failure($"Failed to generate pre-signed URL document {document.DocumentId} for customer {document.CustomerId}.");
            }

            return Result<Document>.Success(new Document
            {
                CustomerId = document.CustomerId,
                OrderCode = document.OrderCode,
                DocumentId = document.DocumentId,
                FileName = documentStorageClient.GetMetadataValue(response.Data, MetadataConstants.FileName),
                DocumentType = documentStorageClient.GetMetadataValue(response.Data, MetadataConstants.DocumentType),
                ContentType = documentStorageClient.GetMetadataValue(response.Data, MetadataConstants.ContentType),
                Url = preSignedUrlresponse.Data,
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get document {DocumentId} for customer {CustomerId}.", document.DocumentId, document.CustomerId);
            return Result<Document>.Failure($"Failed to get document {document.DocumentId} for customer {document.CustomerId}.");
        }
    }

    public async Task<Result<List<Document>>> GetAllDocumentsAsync(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        try
        {
            var response = await documentStorageClient.GetObjectsAsync(document.CustomerId, document.DocumentType);
            if (!response.IsSuccess)
            {
                logger.LogError("Failed to get objects for customer {CustomerId}.", document.CustomerId);
                return Result<List<Document>>.Failure($"Failed to get objects for customer {document.CustomerId}.");
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
            logger.LogError(ex, "Failed to get documents for customer {CustomerId}.", document.CustomerId);
            return Result<List<Document>>.Failure($"Failed to get documents for customer {document.CustomerId}.");
        }
    }
}