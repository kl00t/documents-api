namespace Documents.Api.Dto;

public record GetDocumentRequest(string CustomerId, string OrderCode, string DocumentId);