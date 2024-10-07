using Documents.Service.Models;

namespace Documents.Api.Dto;

public record GetAllDocumentsRequest(string CustomerId, string OrderCode)
{
    public Document ToDomain()
    {
        return new Document
        {
            CustomerId = CustomerId,
            OrderCode = OrderCode
        };
    }
}