using Microsoft.Extensions.DependencyInjection;

namespace Documents.Service.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IDocumentService, DocumentService>();
    }
}