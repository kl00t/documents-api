using Documents.Service.Models;

namespace Documents.Api.Dto;

public record GetAllDocumentsRequest(string CustomerId)
{
    public Document ToDomain()
    {
        return new Document
        {
            CustomerId = CustomerId
        };
    }
}