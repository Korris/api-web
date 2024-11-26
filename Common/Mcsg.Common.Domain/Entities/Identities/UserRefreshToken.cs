using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

public partial class UserRefreshToken : AuditableEntity
{
    public Guid UserId { get; set; }

    [StringLength(Validator.Token.Max)]
    public string? RefreshToken { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
