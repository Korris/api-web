using System.ComponentModel.DataAnnotations;

namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

public class ChangePasswordReq : BaseR
{
    [Required]
    public string OldPassword { get; set; }
    [Required]
    public string NewPassword { get; set; }
    [Required]
    public string ConfirmPassword { get; set; }
}
