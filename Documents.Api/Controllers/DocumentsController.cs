using Documents.Api.Dto;
using Documents.Service;
using Microsoft.AspNetCore.Mvc;

namespace Documents.Api.Controllers;

[ApiController]
[Route("customers")]
public class DocumentsController(
    ILogger<DocumentsController> logger,
    IDocumentService documentService) : ControllerBase
{
    private readonly ILogger<DocumentsController> _logger = logger;
    private readonly IDocumentService _documentService = documentService;

    // GET /customer/{customerId}/documents/{documentId}
    [HttpGet]
    [Route("{customerId}/documents/{documentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDocumentById(
        [FromRoute] string customerId,
        [FromRoute] string documentId)
    {
        var getDocumentRequest = new GetDocumentRequest(customerId, documentId);

        var document = getDocumentRequest.ToDomain();

        var result = await _documentService.GetDocumentAsync(document);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Document {documentId} for customer {customerId} not found.", documentId, customerId);
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Documents {documentId} not found {customerId}.");
        }

        return Ok(result.Data); 
    }

    // POST /customer
    [HttpPost]
    public async Task<IActionResult> StoreDocument(IFormFile file, [FromForm]StoreDocumentRequestDto request)
    {
        if (file == null || file.Length == 0)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"No file uploaded.");
        }

        using Stream stream = file!.OpenReadStream();

        var storeDocumentRequest = new StoreDocumentRequest(
            request.CustomerId,
            request.DocumentId,
            file.FileName,
            file.ContentType,
            request.DocumentType);

        var document = storeDocumentRequest.ToDomain();

        var result = await _documentService.StoreDocumentAsync(stream, document);

        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to store document {DocumentId} for customer {CustomerId}", request.DocumentId, request.CustomerId);
            return BadRequest($"Failed to store document {request.DocumentId} for customer {request.CustomerId}.");
        }

        return CreatedAtAction(
            nameof(GetDocumentById),
            new { document.CustomerId, document.DocumentId },
            value: document);
    }

    // DELETE /customer/{customerId}/documents/{documentId}
    [HttpDelete]
    [Route("{customerId}/documents/{documentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteImage([FromRoute] string customerId, [FromRoute] string documentId)
    {
        var deleteObjectRequest = new DeleteDocumentRequest(customerId, documentId);

        var document = deleteObjectRequest.ToDomain();

        var result = await _documentService.DeleteDocumentAsync(document);

        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to delete document {documentId} for customer {customerId}", documentId, customerId);
            return BadRequest($"Failed to delete document {documentId} for customer {customerId}.");
        }

        return Ok($"Document '{documentId}' for customer '{customerId}' deleted.");
    }
}