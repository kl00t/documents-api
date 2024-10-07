using Documents.Api.Dto;
using Documents.Service;
using Microsoft.AspNetCore.Mvc;

namespace Documents.Api.Controllers;

[ApiController]
[Route("customers")]
public class DocumentsController(
    ILogger<DocumentsController> logger,
    ICommandDocumentService commandDocumentService,
    IQueryDocumentService queryDocumentService) : ControllerBase
{
    private readonly ILogger<DocumentsController> _logger = logger;
    private readonly ICommandDocumentService _command  = commandDocumentService;
    private readonly IQueryDocumentService _query = queryDocumentService;

    // GET/customer/{CustomerId}/orders/{OrderCode}/documents
    [HttpGet]
    [Route($"{{CustomerId}}/orders/{{OrderCode}}/documents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllDocumentForCustomer([FromRoute] GetAllDocumentsRequest request, [FromQuery] string? documentType = null)
    {
        var document = request.ToDomain();
        document.DocumentType = documentType;

        var result = await _query.GetAllDocumentsAsync(document);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Documents for customer {customerId} and order {orderCode} not found.", request.CustomerId, request.OrderCode);
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Documents for customer '{request.CustomerId}' and order '{request.OrderCode}' not found.");
        }

        var response = DocumentResponse.FromDomain(result.Data!);

        return Ok(response);
    }

    // GET /customer/{CustomerId}/orders/{OrderCode}/documents/{DocumentId}
    [HttpGet]
    [Route($"{{CustomerId}}/orders/{{OrderCode}}/documents/{{DocumentId}}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDocumentUrlById([FromRoute] GetDocumentRequest request)
    {
        var document = request.ToDomain();

        var result = await _query.GetDocumentUrlAsync(document);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Document {documentId} for customer {customerId} not found.", request.DocumentId, request.CustomerId);
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Documents {request.DocumentId} not found {request.CustomerId}.");
        }

        var response = GetDocumentResponse.FromDomain(result.Data!);

        return Ok(response);
    }

    // POST /customer
    [HttpPost]
    public async Task<IActionResult> StoreDocument(IFormFile file, [FromForm]StoreDocumentRequest request)
    {
        if (file == null || file.Length == 0)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"No file uploaded.");
        }

        using Stream stream = file!.OpenReadStream();

        var document = request.ToDomain();
        document.FileName = file.FileName;
        document.ContentType = file.ContentType;

        var result = await _command.StoreDocumentAsync(stream, document);
        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to store document for customer '{CustomerId}' and OrderCode {OrderCode}", request.CustomerId, request.OrderCode);
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Failed to store document for customer '{request.CustomerId}' and '{request.OrderCode}'.");
        }

        return CreatedAtAction(
            nameof(GetDocumentUrlById),
            new { result.Data.CustomerId, result.Data.OrderCode, result.Data.DocumentId },
            value: document);
    }

    // DELETE /customer/{CustomerId}/orders/{OrderCode}/documents/{DocumentId}
    [HttpDelete]
    [Route($"{{CustomerId}}/orders/{{OrderCode}}/documents/{{DocumentId}}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteDocument([FromRoute] DeleteDocumentRequest request)
    {
        var document = request.ToDomain();

        var result = await _command.DeleteDocumentAsync(document);

        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to delete document '{documentId}' for customer '{customerId}' and order {orderCode}", request.DocumentId, request.CustomerId, request.OrderCode);
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Failed to delete document '{request.DocumentId}' for customer '{request.CustomerId}' and order {request.OrderCode}.");
        }

        return Ok($"Document '{request.DocumentId}' for customer '{request.CustomerId}' and order '{request.OrderCode}' deleted.");
    }
}