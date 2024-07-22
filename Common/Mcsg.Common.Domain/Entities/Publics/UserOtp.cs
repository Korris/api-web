namespace Mcsg.Common.Domain.Entities;

using Common;
using Core.Enums;

public class UserOtp : AuditableEntity
{
    public Guid UserId { get; set; }
    public UserOtpType OtpType { get; set; }
    public string? Token { get; set; }
    public string? Destination { get; set; }
    public string? Code { get; set; }
    public DateTime? ExpiryTime { get; set; }
}