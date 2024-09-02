using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;
using SeedWork.Constants;

public class Feedback : AuditableEntity
{
    public SatisfactionLevel Satisfaction { get; set; }

    public PostType? Type { get; set; }

    public Guid? UserId { get; set; }

    [StringLength(Validator.Email.Max)]
    public string Email { get; set; } = default!;

    [StringLength(Validator.Description.Max)]
    public string Comment { get; set; } = default!;
}