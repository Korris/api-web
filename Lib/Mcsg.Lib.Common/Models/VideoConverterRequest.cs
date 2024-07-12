namespace Mcsg.Lib.Common.Models;

public enum TargetVideoFormat
{
    MP4,
    WEBM
}

public class VideoConverterRequest
{
    public string Url { get; set; }
    public Guid ResourceId { get; set; }
    public TargetVideoFormat TargetFormat { get; set; } = TargetVideoFormat.MP4;
}
