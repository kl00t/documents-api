using Documents.Client;
using Documents.Client.Constants;
using Documents.Service.Models;
using Microsoft.Extensions.Logging;

namespace Documents.Service;

public class DocumentService(ILogger<DocumentService> logger, IDocumentStorageClient documentStorageClient) : IDocumentService
{
    private static string ConstructKey(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return $"{document.CustomerId}/{document.DocumentId}";
    }

    public async Task<Result<bool>> DeleteDocumentAsync(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        try
        {
            var key = ConstructKey(document);
            var response = await documentStorageClient.DeleteObjectAsync(key);
            return Result<bool>.Success(response.IsSuccess);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete document {DocumentId} for customer {CustomerId}.", document.DocumentId, document.CustomerId);
            return Result<bool>.Failure($"Failed to delete document {document.DocumentId} for customer {document.CustomerId}.");
        }
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

    public async Task<Result<Document>> StoreDocumentAsync(Stream fileStream, Document document)
    {
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentNullException.ThrowIfNull(document);

        try
        {
            document.DocumentId = Guid.NewGuid().ToString();

            string key = ConstructKey(document);

            var response = await documentStorageClient.PutObject(
                fileStream, 
                key, 
                document.FileName!, 
                document.DocumentType!, 
                document.ContentType!);

            if (!response.IsSuccess)
            {
                logger.LogError("Failed to store document {DocumentId} for customer {CustomerId}.", document.DocumentId, document.CustomerId);
                return Result<Document>.Failure($"Failed to store document {document.DocumentId} for customer {document.CustomerId}.");
            }

            return Result<Document>.Success(new Document
            {
                CustomerId = document.CustomerId,
                DocumentId = document.DocumentId,
                FileName = document.FileName,
                DocumentType = document.DocumentType,
                ContentType = document.ContentType
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to store document {DocumentId} for customer {CustomerId}.", document.DocumentId, document.CustomerId);
            return Result<Document>.Failure($"Failed to store document {document.DocumentId} for customer {document.CustomerId}.");
        }
    }
}