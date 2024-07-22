namespace Mcsg.Story.Api.Models;

using Common.Core.Enums;

public class ReactionsUserModel
{
    public ReactionType Type { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string AuthorAvatar { get; set; }
}
