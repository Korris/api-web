namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public class UserRefreshToken : AuditableEntity
{
    public Guid UserId { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}