namespace Documents.Service.Models;

public class Document
{
    public string FileName { get; set; }

    public string ContentType { get; set; }

    public string ContentBody { get; set; }

    public string CustomerId { get; set; }

    public string OrderCode { get; set; }

    public DateTime CreatedAt { get; set; }
}
