using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public partial class Session : AuditableEntity
{
    public string? LoginProvider { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime LoginDateUtc { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime ExpiredDateUtc { get; set; }

    public string? UserName { get; set; }
    public string? ProfileName { get; set; }
    public string? ProfileId { get; set; }
    public string? UserAvatar { get; set; }
    public string? Email { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public Guid UserId { get; set; }
    public string? Roles { get; set; }
    public string? Claims { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime LastActionDateUtc { get; set; }

    public DateOnly? PremiumDate { get; set; }
}
