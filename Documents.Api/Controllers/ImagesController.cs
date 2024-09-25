using Amazon.S3;
using Amazon.S3.Model;
using Documents.Api.Dto;
using Documents.Api.Settings;
using Documents.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Documents.Api.Controllers;

[ApiController]
[Route("images")]
public class ImagesController(
    ILogger<DocumentsController> logger,
    IDocumentService documentService,
    IAmazonS3 s3Client,
    IOptions<S3Settings> s3Settings) : ControllerBase
{
    private readonly ILogger<DocumentsController> _logger = logger;
    private readonly IDocumentService _documentService = documentService;
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly IOptions<S3Settings> _s3Settings = s3Settings;

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        using var stream = file.OpenReadStream();
        var key = $"{Guid.NewGuid().ToString()}-{file.FileName}";
        var putRequest = new PutObjectRequest
        {
            BucketName = _s3Settings.Value.BucketName,
            Key = $"images/{key}",
            InputStream = stream,
            ContentType = file.ContentType,
            Metadata =
                {
                    ["file-name"] = file.FileName
                }
        };

        await _s3Client.PutObjectAsync(putRequest);
        return Ok(key);
    }
}