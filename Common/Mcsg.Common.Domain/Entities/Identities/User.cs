using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork.Enums;

public partial class User : IdentityUser<Guid>
{
    /// <summary>
    /// Use for display name
    /// </summary>
    public string? ProfileName { get; set; }

    /// <summary>
    /// Store the first username
    /// </summary>
    public string? ProfileId { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Gender { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? ReferralCode { get; set; }
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime? ActivedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string? StatusReason { get; set; }
    public string? CoverPhoto { get; set; }
    public string? Location { get; set; }
    public DateOnly? PremiumDate { get; set; }
    public bool IsActiveEarning { get; set; }
    public bool IsWalletShowing { get; set; }

    /// <summary>
    /// 0 Guest, 1 Free, 2 Premium, 3 Administrator
    /// </summary>
    public UserType Type { get; set; }

    /// <summary>
    /// Created IP
    /// </summary>
    [Column(TypeName = "varchar(256)")]
    public string? CreatedIp { get; set; }

    /// <summary>
    /// Last login IP
    /// </summary>
    [Column(TypeName = "varchar(256)")]
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// Created by
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Created on
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Modified by
    /// </summary>
    public Guid? ModifiedBy { get; set; }

    /// <summary>
    /// Modified date
    /// </summary>
    public DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// The scheduled time for the job will be deleted in the future
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Deleted by
    /// </summary>
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// Is delete
    /// </summary>
    public bool IsDelete { get; set; }
}