using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;
using SeedWork.Constants;

public partial class Feedback : AuditableEntity
{
    public FeedbackType Type { get; set; }

    public Guid? UserId { get; set; }

    [StringLength(Validator.Email.Max)]
    public string Email { get; set; } = default!;

    [StringLength(Validator.Description.Max)]
    public string Comment { get; set; } = default!;

    [InverseProperty("Feedback")]
    public virtual ICollection<SystemResource> SystemResources { get; set; } = new List<SystemResource>();

    [ForeignKey("UserId")]
    [InverseProperty("Feedbacks")]
    public virtual User? User { get; set; }
}