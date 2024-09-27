namespace Documents.Service.Models;

public class Document
{
    public required string CustomerId { get; set; }

    public string DocumentId { get; set; }

    public string? FileName { get; set; }

    public string? ContentType { get; set; }

    public string? DocumentType { get; set; }

    public string? Url { get; set; }
}
