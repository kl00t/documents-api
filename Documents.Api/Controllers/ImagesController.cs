using Amazon.S3;
using Amazon.S3.Model;
using Documents.Api.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Documents.Api.Controllers;

[ApiController]
[Route("images")]
public class ImagesController(
    IAmazonS3 s3Client,
    IOptions<S3Settings> s3Settings) : ControllerBase
{
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
        var key = Guid.NewGuid();
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

    [HttpGet("{key}")]
    public async Task<IActionResult> GetImage([FromRoute]string key)
    {
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = _s3Settings.Value.BucketName,
            Key = $"images/{key}"
        };

        var response = await _s3Client.GetObjectAsync(getObjectRequest);

        return File(
            response.ResponseStream,
            response.Headers.ContentType,
            response.Metadata["file-name"]);
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