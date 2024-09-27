using Amazon.S3;
using Amazon.S3.Model;
using Documents.Client.Constants;
using Documents.Client.Settings;
using Documents.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace Documents.Client;

public class DocumentStorageClient(ILogger<DocumentStorageClient> logger, IAmazonS3 s3Client, IOptions<S3Settings> s3Settings) : IDocumentStorageClient
{
    private readonly ILogger<DocumentStorageClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IAmazonS3 _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
    private readonly S3Settings _s3Settings = s3Settings?.Value ?? throw new ArgumentNullException(nameof(s3Settings));

    public string GetMetadataValue(MetadataCollection metadataCollection, string metadataTag)
    {
        if (metadataCollection.Keys.Contains(metadataTag))
        {
            return metadataCollection[metadataTag];
        }

        return string.Empty;
    }

    public async Task<OperationResult<bool>> DeleteObjectAsync(string key)
    {
        return await ExecuteWithLoggingAsync(async () =>
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = key
            };

            var response = await _s3Client.DeleteObjectAsync(deleteObjectRequest).ConfigureAwait(false);
            return response.HttpStatusCode == HttpStatusCode.OK
                ? OperationResult<bool>.Success(response.HttpStatusCode == HttpStatusCode.OK)
                : OperationResult<bool>.Failure($"Failed to delete object for key: {key}", response.HttpStatusCode);

        }, key);
    }

    public async Task<OperationResult<MetadataCollection>> GetObjectMetaData(string key)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectMetadataAsync(request).ConfigureAwait(false);
            return response.HttpStatusCode == HttpStatusCode.OK
                ? OperationResult<MetadataCollection>.Success(response.Metadata)
                : OperationResult<MetadataCollection>.Failure($"Failed to get metadata for key: {key}", response.HttpStatusCode);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching metadata for key {Key}", key);
            return OperationResult<MetadataCollection>.Failure($"Failed to get metadata for key: {key}.", HttpStatusCode.NotFound);
        }
    }

    public async Task<OperationResult<string>> GetPreSignedUrlAsync(string key)
    {
        return await ExecuteWithLoggingAsync(async () =>
        {
            var getPreSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(_s3Settings.PreSignedUrlExpiry)
            };

            var response = await _s3Client.GetPreSignedURLAsync(getPreSignedUrlRequest);
            return !string.IsNullOrEmpty(response)
                ? OperationResult<string>.Success(response)
                : OperationResult<string>.Failure($"Failed to get presigned url for {key}", HttpStatusCode.BadRequest);
        }, key);
    }

    public async Task<OperationResult<bool>> PutObject(Stream stream, string key, string fileName, string documentType, string contentType)
    {
        return await ExecuteWithLoggingAsync(async () =>
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = contentType,
                Metadata =
                {
                    [MetadataConstants.FileName] = fileName,
                    [MetadataConstants.DocumentType] = documentType,
                    [MetadataConstants.ContentType] = contentType
                }
            };

            var response = await _s3Client.PutObjectAsync(putRequest).ConfigureAwait(false);
            return response.HttpStatusCode == HttpStatusCode.OK
                ? OperationResult<bool>.Success(response.HttpStatusCode == HttpStatusCode.OK)
                : OperationResult<bool>.Failure($"Failed to store object for key: {key}", response.HttpStatusCode);
        }, key);
    }

    public async Task<OperationResult<GetDocumentResult>> GetObjectAsync(string key)
    {
        return await ExecuteWithLoggingAsync(async () =>
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(getObjectRequest);
            return response.HttpStatusCode == HttpStatusCode.OK
                ? OperationResult<GetDocumentResult>.Success(new GetDocumentResult(response.ResponseStream, response.Headers.ContentType, response.Metadata[MetadataConstants.FileName]))
                : OperationResult<GetDocumentResult>.Failure($"Failed to get object for key: {key}", response.HttpStatusCode);
        }, key);
    }

    private async Task<T> ExecuteWithLoggingAsync<T>(Func<Task<T>> func, string key)
    {
        try
        {
            return await func().ConfigureAwait(false);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Amazon S3 error occurred for key: {Key}. Error message: {Message}", key, ex.Message);
            throw new S3ClientException("Amazon S3 error occurred. Error message: {Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred for key: {Key}. Error message: {Message}", key, ex.Message);
            throw new S3ClientException("Amazon S3 error occurred. Error message: {Message}", ex);
        }
    }
}