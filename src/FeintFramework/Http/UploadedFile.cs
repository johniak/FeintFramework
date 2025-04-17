namespace FeintFramework.Http;

public class UploadedFile
{
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public required Stream Content { get; set; }
}