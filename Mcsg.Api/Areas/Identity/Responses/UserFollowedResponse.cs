namespace Mcsg.Api.Areas.Identity.Responses
{
    public class UserFollowedResponse
    {
        public Guid UserId { get; set; }
        public string? ProfileName { get; set; }
        public string? UserName { get; set; }
        public string? Avatar { get; set; }
        public bool IsFollowing { get; set; } = false;
    }
}
