using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

public partial class UserNameHistory : AuditableEntity
{
    public Guid UserId { get; set; }

    [StringLength(Validator.UserName.Max)]
    public string? UserName { get; set; }
}
