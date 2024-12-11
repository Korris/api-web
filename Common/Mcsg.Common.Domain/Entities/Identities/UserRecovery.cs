using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

public partial class UserRecovery : AuditableEntity
{
    public Guid UserId { get; set; }

    [StringLength(Validator.Token.Max)]
    public string? SecretKey { get; set; }
}
