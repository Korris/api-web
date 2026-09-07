namespace Mcsg.Api.Areas.Social.Dtos;

using Common.Core.Enums;

public class PostLinkDbDto
{
    public string? HashId { get; set; }
    public string? Url { get; set; }
    public PostLinkType Type { get; set; }
}
