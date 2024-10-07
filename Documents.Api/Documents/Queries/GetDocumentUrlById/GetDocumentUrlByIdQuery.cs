using Documents.Api.Dto;
using MediatR;

namespace Documents.Api.Documents.Queries.GetDocumentUrlById;

public class GetDocumentUrlByIdQuery : IRequest<BaseResponse<GetDocumentResponse>>
{
    public required string CustomerId { get; set; }

    public required string OrderCode { get; set; }

    public required string DocumentId { get; set; }
}