using System.Security.Claims;

namespace Mcsg.Lib.Common.Security.Models;

public class CurrentUserModel
{
    public Guid? UserId { get; set; }
    public IEnumerable<Claim> Claims { get; set; }
    public string? SessionId { get; set; }
}
