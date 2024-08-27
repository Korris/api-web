using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using Enums;
using SeedWork;
using SeedWork.Constants;
using SeedWork.Enums;

public partial class Device : AuditableEntity
{
    /// <summary>
    /// UserId
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    [StringLength(Validator.DeviceToken.Max)]
    public string Token { get; set; } = default!;

    /// <summary>
    /// 1 Other, 2 iOS, 3 Android, 4 Web
    /// </summary>
    public DeviceType Type { get; set; }

    /// <summary>
    /// 0 Guest, 1 Free, 2 Premium, 3 Administrator
    /// </summary>
    public UserType UserType { get; set; }
}
