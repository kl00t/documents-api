using Amazon;
using Amazon.Runtime;
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
            var s3Settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
            var clientConfiguration = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region)
            };

            return new AmazonS3Client(
                new BasicAWSCredentials(s3Settings.AccessKey, 
                s3Settings.SecretKey), clientConfiguration);
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

        app.Run();
    }
}
