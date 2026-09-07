namespace Mcsg.Api.Areas.Identity.Requests;

using Common.Core.Requests;
using Common.Enums;

public class UserProfileUpdateR : BaseR
{
    public string? NewProfileName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderType? Gender { get; set; }
    public string? Location { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsWalletShowing { get; set; }
}
