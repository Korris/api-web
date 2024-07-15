namespace Mcsg.Social.Api.Models;

public class UserSearchResponse
{
    public string ProfileId { get; set; }
    public string ProfileName { get; set; }
    public Guid Id { get; set; }
    public string Avatar { get; set; }
    public bool IsFollowing { get; set; }

}
