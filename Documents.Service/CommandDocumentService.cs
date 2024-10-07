using Documents.Client;
using Documents.Service.Models;
using Microsoft.Extensions.Logging;

namespace Documents.Service;

public class CommandDocumentService(ILogger<CommandDocumentService> logger, IDocumentStorageClient documentStorageClient) : ICommandDocumentService
{
    private static string ConstructKey(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);
        return $"{document.CustomerId}/{document.OrderCode}/{document.DocumentId}";
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
                OrderCode = document.OrderCode,
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
