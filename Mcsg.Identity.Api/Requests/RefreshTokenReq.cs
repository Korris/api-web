using System.ComponentModel.DataAnnotations;

namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

public class RefreshTokenReq : BaseR
{
    [Required]
    public string RefreshToken { get; set; }
}
