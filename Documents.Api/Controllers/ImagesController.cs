using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Model.Internal.MarshallTransformations;
using Documents.Api.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;

namespace Documents.Api.Controllers;

public class GetAllImagesResponse
{
    public  MyImage[] Images { get; set; }
}

public class MyImage
{
    public string FileName { get; set; }

    public string Key { get; set; }

    public string ContentType { get; set; }

    public string DocumentType { get; set; }
}

public class DocumentUploadRequest
{
    public string FileName { get; set; }

    public string ContentType { get; set; }

    public string DocumentType { get; set; }
}

public static class Constants
{
    public static class DocumentType
    {
        public const string PDF = "pdf";
        public const string CONFIRMATION = "confirmation";
        public const string EMAIL = "email";
    }
}

[ApiController]
[Route("images")]
public class ImagesController(
    IAmazonS3 s3Client,
    IOptions<S3Settings> s3Settings) : ControllerBase
{
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly IOptions<S3Settings> _s3Settings = s3Settings;

    [HttpPost("/presigned")]
    public async Task<IActionResult> UploadImagePresigned(IFormFile file, [FromForm] DocumentUploadRequest request)
    {
        var key = Guid.NewGuid();
        var getPreSignedUrlRequest = new GetPreSignedUrlRequest
        {
            BucketName = _s3Settings.Value.BucketName,
            Key = $"images/{key}",
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(10),
            ContentType = request.ContentType,
            Metadata =
            {
                ["file-name"] = request.FileName,
                ["document-type"] = request.DocumentType
            }
        };

        var preSignedUrl = await _s3Client.GetPreSignedURLAsync(getPreSignedUrlRequest);

        return Ok(new { key, url = preSignedUrl });
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromForm]string documentType)
    {
        if (file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        using var stream = file.OpenReadStream();
        var key = Guid.NewGuid();
        var putRequest = new PutObjectRequest
        {
            BucketName = _s3Settings.Value.BucketName,
            Key = $"images/{key}",
            InputStream = stream,
            ContentType = file.ContentType,
            Metadata =
            {
                ["file-name"] = file.FileName,
                ["document-type"] = documentType,
                ["content-type"] = file.ContentType
            }
        };

        await _s3Client.PutObjectAsync(putRequest);
        return Ok(key);
    }

    private async Task<MetadataCollection> GetObjectMetaData(string key)
    {
        var request = new GetObjectMetadataRequest
        {
            BucketName = _s3Settings.Value.BucketName,
            Key = key,
        };

        var response = await _s3Client.GetObjectMetadataAsync(request);
        return response.Metadata;
    }

    private string GetMetadataValue(MetadataCollection metadataCollection, string metadataTag)
    {
        if (metadataCollection.Keys.Contains(metadataTag))
        {
            return metadataCollection[metadataTag];
        }

        return string.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllImages([FromQuery]string? documentType = null)
    {
        var request = new ListObjectsV2Request
        {
            BucketName = _s3Settings.Value.BucketName,
        };

        var response = await _s3Client.ListObjectsV2Async(request);
        var images = new List<MyImage>();

        foreach (var s3Object in response.S3Objects)
        {
            var metadataCollection = await GetObjectMetaData(s3Object.Key);

            var objectDocumentType = GetMetadataValue(metadataCollection, "x-amz-meta-document-type");

            if (documentType == null || objectDocumentType == documentType)
            {
                images.Add(new MyImage
                {
                    FileName = GetMetadataValue(metadataCollection, "x-amz-meta-file-name"),
                    Key = s3Object.Key,
                    ContentType = GetMetadataValue(metadataCollection, "x-amz-meta-content-type"),
                    DocumentType = objectDocumentType
                });
            }
        }

        var getAllImagesResponse = new GetAllImagesResponse
        {
            Images = images.ToArray()
        };

        return Ok(getAllImagesResponse);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetImage([FromRoute]string key)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            { 
                BucketName = _s3Settings.Value.BucketName,
                Key = $"images/{key}",
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(10)
            };

            string preSignedUrl = await _s3Client.GetPreSignedURLAsync(request);

            return Ok(new { key, url = preSignedUrl });
        }
        catch (AmazonS3Exception ex)
        {
            return BadRequest($"S3 error generating pre-signed URL: {ex.Message}");
        }
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> DeleteImage([FromRoute]string key)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _s3Settings.Value.BucketName,
            Key = $"images/{key}"
        };

        var response = await _s3Client.DeleteObjectAsync(deleteObjectRequest);

        return Ok($"File {key} deleted.");
    }
}