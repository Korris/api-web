using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public partial class UserReferral : AuditableEntity
{
    public Guid UserReferrerId { get; set; }
    public Guid UserRefereeId { get; set; }

    [ForeignKey("UserRefereeId")]
    [InverseProperty("UserReferralUserReferees")]
    public virtual User UserReferee { get; set; } = null!;

    [ForeignKey("UserReferrerId")]
    [InverseProperty("UserReferralUserReferrers")]
    public virtual User UserReferrer { get; set; } = null!;
}
