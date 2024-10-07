using Documents.Api.Dto;
using Documents.Service;
using MediatR;

namespace Documents.Api.Documents.Queries.GetDocumentUrlById;

public class GetDocumentUrlByIdQueryHandler(IQueryDocumentService queryDocumentService) : IRequestHandler<GetDocumentUrlByIdQuery, BaseResponse<GetDocumentResponse>>
{
    public async Task<BaseResponse<GetDocumentResponse>> Handle(GetDocumentUrlByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await queryDocumentService.GetDocumentUrlAsync(
            request.CustomerId,
            request.OrderCode, 
            request.DocumentId);

        return result.IsSuccess && result.Data is not null
            ? BaseResponse<GetDocumentResponse>.Success(GetDocumentResponse.FromDomain(result.Data))
            : BaseResponse<GetDocumentResponse>.Failure("Failed to retrieve document URL.");
    }
}
