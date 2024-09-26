namespace Documents.Api.Dto;

public record StoreDocumentRequestDto(
    string CustomerId, 
    string DocumentId, 
    string DocumentType);