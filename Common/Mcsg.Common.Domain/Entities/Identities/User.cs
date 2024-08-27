using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork.Constants;
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

    [Column(TypeName = "timestamp")]
    public DateTime? DateOfBirth { get; set; }

    public int? Gender { get; set; }
    public string? RefreshToken { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public string? ReferralCode { get; set; }
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;

    [Column(TypeName = "timestamp")]
    public DateTime? ActivedDate { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? LastLoginDate { get; set; }

    [StringLength(Validator.Description.Max)]
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
    [StringLength(Validator.Ip.Max)]
    public string? CreatedIp { get; set; }

    /// <summary>
    /// Last login IP
    /// </summary>
    [StringLength(Validator.Ip.Max)]
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// Created by
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Created on
    /// </summary>
    [Column(TypeName = "timestamp")]
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Modified by
    /// </summary>
    public Guid? ModifiedBy { get; set; }

    /// <summary>
    /// Modified date
    /// </summary>
    [Column(TypeName = "timestamp")]
    public DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// The scheduled time for the job will be deleted in the future
    /// </summary>
    [Column(TypeName = "timestamp")]
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