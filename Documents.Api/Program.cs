using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Documents.Api.Settings;
using Documents.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Documents.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors();
        builder.Services.AddScoped<IDocumentService, DocumentService>();
        
        // Configure S3
        builder.Services.Configure<S3Settings>(builder.Configuration.GetSection("S3Settings"));
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(settings.Region)
            };
            return new AmazonS3Client(config);
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        //app.MapPost("images", async (
        //    [FromForm] IFormFile file,
        //    IAmazonS3 s3Client,
        //    IOptions<S3Settings> settings) =>
        //{
        //    if (file.Length == 0)
        //    {
        //        return Results.BadRequest("No file uploaded");
        //    }

        //    using var stream = file.OpenReadStream();
        //    var key = $"{Guid.NewGuid().ToString()}-{file.FileName}";
        //    var putRequest = new PutObjectRequest
        //    {
        //        BucketName = settings.Value.BucketName,
        //        Key = $"images/{key}",
        //        InputStream = stream,
        //        ContentType = file.ContentType,
        //        Metadata =
        //        {
        //            ["file-name"] = file.FileName
        //        }
        //    };

        //    await s3Client.PutObjectAsync(putRequest);
        //    return Results.Ok(key);
        //}).DisableAntiforgery();

        app.Run();
    }
}
