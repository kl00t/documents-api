using Amazon;
using Amazon.S3;
using Documents.Client.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Documents.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddS3Client(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDocumentStorageClient, DocumentStorageClient>();

        services.Configure<S3Settings>(options => configuration.GetSection("S3Settings").Bind(options));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var s3Settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
            var clientConfiguration = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region)
            };

            return new AmazonS3Client(clientConfiguration);
        });
    }
}