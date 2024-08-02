namespace Mcsg.Social.Api.Models;

public class ReportReferralResponse
{
    public Guid UserId { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ProfileName { get; set; }
    public string? UserName { get; set; }
    public string? Avatar { get; set; }
}
