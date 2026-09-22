namespace Mcsg.Api.Areas.TapShow.Dtos;

using Common.Core.Enums;

/// <summary>
/// Result of a TapShow image upload (post thumbnail or segment image)
/// </summary>
public class UploadFileDto
{
    public string HashId { get; set; } = default!;
    public string Url { get; set; } = default!;
    public string Name { get; set; } = default!;
    public ResourceType Type { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double Size { get; set; }
}
