namespace Mcsg.Social.Api.Models;

using Common.Core.Enums;

public class ReactionsUserModel
{
    public ReactionType Type { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string AuthorAvatar { get; set; }
    public string? UserName { get; set; }
    public bool? IsFollowing { get; set; }
    public bool IsDeletedUser { get; set; }
}
