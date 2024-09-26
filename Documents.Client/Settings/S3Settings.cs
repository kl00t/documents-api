namespace Documents.Client.Settings;

public class S3Settings
{
    public string Region { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int PreSignedUrlExpiry { get; set; } = 10;
}