using Documents.Api.Dto;
using Documents.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Documents.Api.Controllers;

[ApiController]
[Route("customers/{customerId}/documents")]
public class DocumentsController : ControllerBase
{
    private readonly ILogger<DocumentsController> _logger;
    private readonly IDocumentService _documentService;

    public DocumentsController(ILogger<DocumentsController> logger, IDocumentService documentService)
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
        return Ok();
        //var documents = await _documentService.GetDocumentsByCustomerIdAsync(customerId, documentType);
        //if (documents == null || documents.Count == 0)
        //{
        //    return NotFound(new
        //    {
        //        message = $"No documents found for customer ID {customerId}" +
        //    (documentType != null ? $" and document type {documentType}" : "")
        //    });
        //}
        //return Ok(documents);
    }

    // GET /customers/{customerId}/orders/{orderCode}/documents?documentType=image/png
    [HttpGet("orders/{orderCode}")]
    public async Task<IActionResult> GetDocumentsByCustomerIdAndOrderCode(
        [FromRoute] string customerId,
        [FromRoute] string orderCode,
        [FromQuery] string documentType = null)
    {
        return Ok();
        //var documents = await _documentService.GetDocumentsByCustomerAndOrderAsync(customerId, orderCode, documentType);
        //if (documents == null || documents.Count == 0)
        //{
        //    return NotFound(new
        //    {
        //        message = $"No documents found for customer ID {customerId}, order code {orderCode}" +
        //    (documentType != null ? $", and document type {documentType}" : "")
        //    });
        //}
        //return Ok(documents);
    }

    // POST /customers/{customerId}/documents
    [HttpPost]
    public async Task<IActionResult> StoreDocumentForCustomer(
        [FromRoute] string customerId,
        [FromBody] DocumentDto documentDto)
    {
        return Ok();
        //if (customerId != documentDto.CustomerId)
        //{
        //    return BadRequest(new { message = "Customer ID mismatch." });
        //}

        //var newDocument = await _documentService.StoreDocumentAsync(documentDto);
        //if (newDocument == null)
        //{
        //    return BadRequest(new { message = "Invalid document data." });
        //}

        //return CreatedAtAction(nameof(GetDocumentsByCustomerId),
        //    new { customerId = newDocument.CustomerId },
        //    newDocument);
    }

    // POST /customers/{customerId}/orders/{orderCode}/documents
    [HttpPost("orders/{orderCode}")]
    public async Task<IActionResult> StoreDocumentForOrder(
        [FromRoute] string customerId,
        [FromRoute] string orderCode,
        [FromBody] DocumentDto documentDto)
    {
        return Ok();
        //if (customerId != documentDto.CustomerId || orderCode != documentDto.OrderCode)
        //{
        //    return BadRequest(new { message = "Customer ID or Order Code mismatch." });
        //}

        //var newDocument = await _documentService.StoreDocumentAsync(documentDto);
        //if (newDocument == null)
        //{
        //    return BadRequest(new { message = "Invalid document data." });
        //}

        //return CreatedAtAction(nameof(GetDocumentsByCustomerIdAndOrderCode),
        //    new { customerId = newDocument.CustomerId, orderCode = newDocument.OrderCode },
        //    newDocument);
    }

}
