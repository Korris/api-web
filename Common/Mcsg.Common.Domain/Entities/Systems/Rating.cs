using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;
using SeedWork.Constants;

public partial class Rating : AuditableEntity
{
    public SatisfactionLevel Satisfaction { get; set; }

    public Guid? UserId { get; set; }

    [StringLength(Validator.Email.Max)]
    public string Email { get; set; } = default!;

    [StringLength(Validator.Description.Max)]
    public string Comment { get; set; } = default!;

    [ForeignKey("UserId")]
    [InverseProperty("Ratings")]
    public virtual User? User { get; set; }
}