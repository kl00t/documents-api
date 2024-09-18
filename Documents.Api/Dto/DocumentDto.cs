namespace Documents.Api.Dto;

public class DocumentDto
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public string CustomerId { get; set; }
    public string OrderCode { get; set; }
    public string Content { get; set; } // Assuming Base64 encoded file content
}
