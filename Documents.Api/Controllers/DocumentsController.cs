using Amazon.S3;
using Amazon.S3.Model;
using Documents.Api.Dto;
using Documents.Api.Settings;
using Documents.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Documents.Api.Controllers;

[ApiController]
[Route("customers/{customerId}/documents")]
public class DocumentsController : ControllerBase
{
    private readonly ILogger<DocumentsController> _logger;
    private readonly IDocumentService _documentService;

    public DocumentsController(
        ILogger<DocumentsController> logger, 
        IDocumentService documentService)
    {
        _logger = logger;
        _documentService = documentService;
    }

    // GET /customers/{customerId}/documents?documentType=application/pdf
    [HttpGet]
    public async Task<IActionResult> GetDocumentsByCustomerId(
        [FromRoute] string customerId,
        [FromQuery] string documentType = null)
    {
        var documents = await _documentService.GetDocumentsByCustomerIdAsync(customerId, documentType);

        return documents is null || documents.Count == 0
            ? Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Documents not found {customerId}.")
            : Ok(DocumentResponse.FromDomain(documents));
    }

    // GET /customers/{customerId}/orders/{orderCode}/documents?documentType=image/png
    [HttpGet("orders/{orderCode}")]
    public async Task<IActionResult> GetDocumentsByCustomerIdAndOrderCode(
        [FromRoute] string customerId,
        [FromRoute] string orderCode,
        [FromQuery] string documentType = null)
    {
        var documents = await _documentService.GetDocumentsByCustomerAndOrderAsync(customerId, orderCode, documentType);

        return documents is null || documents.Count == 0
            ? Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Documents not found {customerId} and {orderCode}.")
            : Ok(DocumentResponse.FromDomain(documents));
    }

    // POST /customers/{customerId}/documents
    [HttpPost]
    public async Task<IActionResult> StoreDocumentForCustomer(
        [FromRoute] string customerId,
        [FromBody] StoreDocumentRequest request)
    {
        var document = request.ToDomain();

        await _documentService.StoreDocumentAsync(document);

        return CreatedAtAction(
            nameof(GetDocumentsByCustomerId), 
            new { document.FileName  }, 
            value: DocumentResponse.FromDomain(document));
    }

    // POST /customers/{customerId}/orders/{orderCode}/documents
    [HttpPost("orders/{orderCode}")]
    public async Task<IActionResult> StoreDocumentForOrder(
        [FromRoute] string customerId,
        [FromRoute] string orderCode,
        [FromBody] StoreDocumentRequest request)
    {
        var document = request.ToDomain();

        await _documentService.StoreDocumentAsync(document);

        return CreatedAtAction(
            nameof(GetDocumentsByCustomerId),
            new { document.FileName },
            value: DocumentResponse.FromDomain(document));
    }

}
