namespace Mcsg.Identity.Api.Requests;

using Lib.Common.Enums;

public class UserProfileUpdateR
{
    public string? ProfileName { get; set; }
    public string? UserName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderEnum? Gender { get; set; }
    public string? Location { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsPremium { get; set; }
    public bool IsWalletShowing { get; set; }
}
