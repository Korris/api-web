namespace Mcsg.Social.Api.Dtos;

using Common.Core.Enums;

public class PostLinkDto
{
    public string? HashId { get; set; }
    public string? Url { get; set; }
    public string? Type { get; set; }
    public PostLinkType PostLinkType { get; set; }
}
