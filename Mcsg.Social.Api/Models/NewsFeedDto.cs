using Mcsg.Common.Core.Enums;

namespace Mcsg.Social.Api.Models;

public class NewsFeedDto
{
    public string? UserAvatar { get; set; }
    public string? UserName { get; set; }
    public string? Body { get; set; }
    public string? HashId { get; set; }
    public string? HashPostId { get; set; }
    public DateTime CreatedDate { get; set; }
    public PostType Type { get; set; }
    public Guid Id { get; set; }
}

