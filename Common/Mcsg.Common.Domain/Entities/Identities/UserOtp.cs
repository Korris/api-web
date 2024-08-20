using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class UserOtp : AuditableEntity
{
    public Guid UserId { get; set; }
    public UserOtpType OtpType { get; set; }
    public string? Token { get; set; }
    public string? Destination { get; set; }
    public string? Code { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? ExpiryTime { get; set; }
}