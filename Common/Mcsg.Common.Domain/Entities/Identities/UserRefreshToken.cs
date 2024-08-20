using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public class UserRefreshToken : AuditableEntity
{
    public Guid UserId { get; set; }
    public string? RefreshToken { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? RefreshTokenExpiryTime { get; set; }
}