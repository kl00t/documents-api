namespace Documents.Api.Models;

public class Document
{
    public string Id { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public string CustomerId { get; set; }
    public string OrderCode { get; set; }
    public DateTime CreatedAt { get; set; }
}
