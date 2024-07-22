namespace Mcsg.Social.Api.Models;

using Common.Core.Enums;

public class NewsFeedDto
{
    public string? UserAvatar { get; set; }
    public string? UserName { get; set; }
    public string? Body { get; set; }
    public string? HashId { get; set; }
    public string? HashPostId { get; set; }
    public string? ProfileName { get; set; }
    public bool IsSubPost { get; set; }
    public DateTime CreatedOn { get; set; }
    public PostType Type { get; set; }
    public Guid Id { get; set; }
    public int? Order { get; set; }
}

