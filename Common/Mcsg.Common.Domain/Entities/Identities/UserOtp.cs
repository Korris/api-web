using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;
using SeedWork.Constants;

public partial class UserOtp : AuditableEntity
{
    public Guid UserId { get; set; }
    public UserOtpType OtpType { get; set; }

    [StringLength(Validator.Token.Max)]
    public string? Token { get; set; }

    [StringLength(Validator.Title.Max)]
    public string? Destination { get; set; }

    [StringLength(Validator.Code.Max)]
    public string? Code { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? ExpiryTime { get; set; }
}
