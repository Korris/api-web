namespace Mcsg.Api.Areas.Game.Dtos;

using Common.Core.Enums;

/// <summary>
/// Result of a game thumbnail / game file upload
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
