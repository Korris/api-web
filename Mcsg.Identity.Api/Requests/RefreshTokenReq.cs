using System.ComponentModel.DataAnnotations;

namespace Mcsg.Identity.Api.Requests;

public class RefreshTokenReq
{
    [Required]
    public string RefreshToken { get; set; }
}
