using System.ComponentModel.DataAnnotations;
namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

public partial class UserSocial : AuditableEntity
{
    public Guid UserId { get; set; }

    [StringLength(Validator.Token.Max)]
    public string? SocialId { get; set; }

    [StringLength(Validator.Token.Max)]
    public string? Type { get; set; }

    [StringLength(Validator.Email.Max)]
    public string? Email { get; set; }

    [StringLength(Validator.Email.Max)]
    public string? PhoneNumber { get; set; }

    [StringLength(Validator.Name.Max)]
    public string? FirstName { get; set; }

    [StringLength(Validator.Name.Max)]
    public string? LastName { get; set; }

    public bool IsRegisterBySocial { get; set; }

    [StringLength(Validator.Token.Max)]
    public string? RegisterBySocialPlatform { get; set; }
}
