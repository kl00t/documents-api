using Amazon.S3.Model;

namespace Documents.Client;

public interface IDocumentStorageClient
{
    Task<OperationResult<string>> GetPreSignedUrlAsync(string key);

    Task<OperationResult<MetadataCollection>> GetObjectMetaData(string key);

    Task<OperationResult<bool>> PutObject(Stream stream, string key, string fileName, string documentType, string contentType);

    Task<OperationResult<bool>> DeleteObjectAsync(string key);

    string GetMetadataValue(MetadataCollection metadataCollection, string metadataTag);

    Task<OperationResult<List<GetDocumentResult>>> GetObjectsAsync(string prefix, string? documentType = null);
}