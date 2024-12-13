using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;
using SeedWork.Constants;

public partial class UserAuthenticator : AuditableEntity
{
    public Guid UserId { get; set; }

    public AuthenticatorType Type { get; set; }

    [StringLength(Validator.Token.Max)]
    public string? SecretKey { get; set; }

    public bool IsActive { get; set; }

    public bool IsLogin { get; set; }

    public bool IsTransaction { get; set; }
}
