using Microsoft.AspNetCore.Identity;

namespace Mcsg.Common.Domain.Entities
{
    using Core.Enums;

    public partial class User : IdentityUser<Guid>
    {
        /// <summary>
        /// Use for display name
        /// </summary>
        public string? ProfileName { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? ReferralCode { get; set; }
        public string? Avatar { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public bool IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTime? ActivedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? StatusReason { get; set; }
        public string? CoverPhoto { get; set; }
        public string? Location { get; set; }

        /// <summary>
        /// Store the first username
        /// </summary>
        public string? ProfileId { get; set; }

        public DateOnly? PremiumDate { get; set; }
        public bool IsActiveEarning { get; set; }

        public User()
        {
            var now = DateTime.UtcNow;
            CreatedDate = now;
            LastModifiedDate = now;
            IsDelete = false;
        }
    }
}