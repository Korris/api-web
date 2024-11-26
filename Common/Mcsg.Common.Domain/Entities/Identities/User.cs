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
    [StringLength(Validator.ProfileName.Max)]
    public string? ProfileId { get; set; }

    [StringLength(Validator.Name.Max)]
    public string? FirstName { get; set; }

    [StringLength(Validator.Name.Max)]
    public string? LastName { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? DateOfBirth { get; set; }

    public int? Gender { get; set; }
    public string? RefreshToken { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? RefreshTokenExpiryTime { get; set; }

    [StringLength(Validator.TagData.Max)]
    public string? ReferralCode { get; set; }

    [StringLength(Validator.Url.Max)]
    public string? Avatar { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Active;

    [Column(TypeName = "timestamp")]
    public DateTime? ActivedDate { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? LastLoginDate { get; set; }

    [StringLength(Validator.Description.Max)]
    public string? StatusReason { get; set; }

    [StringLength(Validator.Url.Max)]
    public string? CoverPhoto { get; set; }

    [StringLength(Validator.Location.Max)]
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

    /// <summary>
    /// Minio instance
    /// </summary>
    public MinioInstanceType MinioInstance { get; set; }

    /// <summary>
    /// Storage limit (MB)
    /// </summary>
    public int StorageLimit { get; set; }

    /// <summary>
    /// Synced on
    /// </summary>
    [Column(TypeName = "timestamp")]
    public DateTime? SyncedOn { get; set; }

    /// <summary>
    /// Sync error
    /// </summary>
    [StringLength(Validator.Description.Max)]
    public string? SyncError { get; set; }

    /// <summary>
    /// Example: ;video;link;11;1;
    /// </summary>
    [StringLength(Validator.TagData.Max)]
    public string? TagData { get; set; }

    [InverseProperty("Author")]
    public virtual ICollection<ComicPostCommentReaction> ComicPostCommentReactions { get; set; } = new List<ComicPostCommentReaction>();

    [InverseProperty("Author")]
    public virtual ICollection<ComicPostComment> ComicPostComments { get; set; } = new List<ComicPostComment>();

    [InverseProperty("User")]
    public virtual ICollection<ComicPostFavorite> ComicPostFavorites { get; set; } = new List<ComicPostFavorite>();

    [InverseProperty("User")]
    public virtual ICollection<ComicPostHide> ComicPostHides { get; set; } = new List<ComicPostHide>();

    [InverseProperty("Author")]
    public virtual ICollection<ComicPostReaction> ComicPostReactions { get; set; } = new List<ComicPostReaction>();

    [InverseProperty("User")]
    public virtual ICollection<ComicPostShare> ComicPostShares { get; set; } = new List<ComicPostShare>();

    [InverseProperty("User")]
    public virtual ICollection<ComicPost> ComicPosts { get; set; } = new List<ComicPost>();

    [InverseProperty("User")]
    public virtual ICollection<ComicReportDetail> ComicReportDetails { get; set; } = new List<ComicReportDetail>();

    [InverseProperty("Author")]
    public virtual ICollection<ComicResource> ComicResources { get; set; } = new List<ComicResource>();

    [InverseProperty("Author")]
    public virtual ICollection<ComicSubPostCommentReaction> ComicSubPostCommentReactions { get; set; } = new List<ComicSubPostCommentReaction>();

    [InverseProperty("Author")]
    public virtual ICollection<ComicSubPostComment> ComicSubPostComments { get; set; } = new List<ComicSubPostComment>();

    [InverseProperty("Author")]
    public virtual ICollection<ComicSubPostReaction> ComicSubPostReactions { get; set; } = new List<ComicSubPostReaction>();

    [InverseProperty("User")]
    public virtual ICollection<ComicSubPost> ComicSubPosts { get; set; } = new List<ComicSubPost>();

    [InverseProperty("Author")]
    public virtual ICollection<DocumentPostCommentReaction> DocumentPostCommentReactions { get; set; } = new List<DocumentPostCommentReaction>();

    [InverseProperty("Author")]
    public virtual ICollection<DocumentPostComment> DocumentPostComments { get; set; } = new List<DocumentPostComment>();

    [InverseProperty("User")]
    public virtual ICollection<DocumentPostFavorite> DocumentPostFavorites { get; set; } = new List<DocumentPostFavorite>();

    [InverseProperty("User")]
    public virtual ICollection<DocumentPostHide> DocumentPostHides { get; set; } = new List<DocumentPostHide>();

    [InverseProperty("Author")]
    public virtual ICollection<DocumentPostReaction> DocumentPostReactions { get; set; } = new List<DocumentPostReaction>();

    [InverseProperty("User")]
    public virtual ICollection<DocumentPostShare> DocumentPostShares { get; set; } = new List<DocumentPostShare>();

    [InverseProperty("User")]
    public virtual ICollection<DocumentPost> DocumentPosts { get; set; } = new List<DocumentPost>();

    [InverseProperty("User")]
    public virtual ICollection<DocumentReportDetail> DocumentReportDetails { get; set; } = new List<DocumentReportDetail>();

    [InverseProperty("Author")]
    public virtual ICollection<DocumentResource> DocumentResources { get; set; } = new List<DocumentResource>();

    [InverseProperty("Author")]
    public virtual ICollection<DocumentSubPostCommentReaction> DocumentSubPostCommentReactions { get; set; } = new List<DocumentSubPostCommentReaction>();

    [InverseProperty("Author")]
    public virtual ICollection<DocumentSubPostComment> DocumentSubPostComments { get; set; } = new List<DocumentSubPostComment>();

    [InverseProperty("Author")]
    public virtual ICollection<DocumentSubPostReaction> DocumentSubPostReactions { get; set; } = new List<DocumentSubPostReaction>();

    [InverseProperty("User")]
    public virtual ICollection<DocumentSubPost> DocumentSubPosts { get; set; } = new List<DocumentSubPost>();

    [InverseProperty("User")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("Receiver")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("User")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    [InverseProperty("User")]
    public virtual ICollection<SmartLookupUser> SmartLookupUsers { get; set; } = new List<SmartLookupUser>();

    [InverseProperty("Author")]
    public virtual ICollection<SocialPostCommentReaction> SocialPostCommentReactions { get; set; } = new List<SocialPostCommentReaction>();

    [InverseProperty("Author")]
    public virtual ICollection<SocialPostComment> SocialPostComments { get; set; } = new List<SocialPostComment>();

    [InverseProperty("User")]
    public virtual ICollection<SocialPostFavorite> SocialPostFavorites { get; set; } = new List<SocialPostFavorite>();

    [InverseProperty("User")]
    public virtual ICollection<SocialPostHide> SocialPostHides { get; set; } = new List<SocialPostHide>();

    [InverseProperty("Author")]
    public virtual ICollection<SocialPostReaction> SocialPostReactions { get; set; } = new List<SocialPostReaction>();

    [InverseProperty("User")]
    public virtual ICollection<SocialPostShare> SocialPostShares { get; set; } = new List<SocialPostShare>();

    [InverseProperty("User")]
    public virtual ICollection<SocialPost> SocialPosts { get; set; } = new List<SocialPost>();

    [InverseProperty("User")]
    public virtual ICollection<SocialReportDetail> SocialReportDetails { get; set; } = new List<SocialReportDetail>();

    [InverseProperty("Author")]
    public virtual ICollection<SocialResource> SocialResources { get; set; } = new List<SocialResource>();

    [InverseProperty("Author")]
    public virtual ICollection<SocialSubPostCommentReaction> SocialSubPostCommentReactions { get; set; } = new List<SocialSubPostCommentReaction>();

    [InverseProperty("Author")]
    public virtual ICollection<SocialSubPostComment> SocialSubPostComments { get; set; } = new List<SocialSubPostComment>();

    [InverseProperty("Author")]
    public virtual ICollection<SocialSubPostReaction> SocialSubPostReactions { get; set; } = new List<SocialSubPostReaction>();

    [InverseProperty("User")]
    public virtual ICollection<SocialSubPost> SocialSubPosts { get; set; } = new List<SocialSubPost>();

    [InverseProperty("Author")]
    public virtual ICollection<StoryPostCommentReaction> StoryPostCommentReactions { get; set; } = new List<StoryPostCommentReaction>();

    [InverseProperty("Author")]
    public virtual ICollection<StoryPostComment> StoryPostComments { get; set; } = new List<StoryPostComment>();

    [InverseProperty("User")]
    public virtual ICollection<StoryPostFavorite> StoryPostFavorites { get; set; } = new List<StoryPostFavorite>();

    [InverseProperty("User")]
    public virtual ICollection<StoryPostHide> StoryPostHides { get; set; } = new List<StoryPostHide>();

    [InverseProperty("Author")]
    public virtual ICollection<StoryPostReaction> StoryPostReactions { get; set; } = new List<StoryPostReaction>();

    [InverseProperty("User")]
    public virtual ICollection<StoryPostShare> StoryPostShares { get; set; } = new List<StoryPostShare>();

    [InverseProperty("User")]
    public virtual ICollection<StoryPost> StoryPosts { get; set; } = new List<StoryPost>();

    [InverseProperty("User")]
    public virtual ICollection<StoryReportDetail> StoryReportDetails { get; set; } = new List<StoryReportDetail>();

    [InverseProperty("Author")]
    public virtual ICollection<StoryResource> StoryResources { get; set; } = new List<StoryResource>();

    [InverseProperty("Author")]
    public virtual ICollection<StorySubPostCommentReaction> StorySubPostCommentReactions { get; set; } = new List<StorySubPostCommentReaction>();

    [InverseProperty("Author")]
    public virtual ICollection<StorySubPostComment> StorySubPostComments { get; set; } = new List<StorySubPostComment>();

    [InverseProperty("Author")]
    public virtual ICollection<StorySubPostReaction> StorySubPostReactions { get; set; } = new List<StorySubPostReaction>();

    [InverseProperty("User")]
    public virtual ICollection<StorySubPost> StorySubPosts { get; set; } = new List<StorySubPost>();

    [InverseProperty("User")]
    public virtual ICollection<SystemSettingHistory> SystemSettingHistories { get; set; } = new List<SystemSettingHistory>();

    [InverseProperty("User")]
    public virtual ICollection<TagFavorite> TagFavorites { get; set; } = new List<TagFavorite>();

    [InverseProperty("Author")]
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    [InverseProperty("UserId1Navigation")]
    public virtual ICollection<UserBlock> UserBlockUserId1Navigations { get; set; } = new List<UserBlock>();

    [InverseProperty("UserId2Navigation")]
    public virtual ICollection<UserBlock> UserBlockUserId2Navigations { get; set; } = new List<UserBlock>();

    [InverseProperty("User")]
    public virtual ICollection<UserExclusiveSubPost> UserExclusiveSubPosts { get; set; } = new List<UserExclusiveSubPost>();

    [InverseProperty("UserFollower")]
    public virtual ICollection<UserFollow> UserFollowUserFollowers { get; set; } = new List<UserFollow>();

    [InverseProperty("UserFollowing")]
    public virtual ICollection<UserFollow> UserFollowUserFollowings { get; set; } = new List<UserFollow>();

    [InverseProperty("UserReferee")]
    public virtual ICollection<UserReferral> UserReferralUserReferees { get; set; } = new List<UserReferral>();

    [InverseProperty("UserReferrer")]
    public virtual ICollection<UserReferral> UserReferralUserReferrers { get; set; } = new List<UserReferral>();

    [InverseProperty("UserId1Navigation")]
    public virtual ICollection<UserRelation> UserRelationUserId1Navigations { get; set; } = new List<UserRelation>();

    [InverseProperty("UserId2Navigation")]
    public virtual ICollection<UserRelation> UserRelationUserId2Navigations { get; set; } = new List<UserRelation>();

    [InverseProperty("User")]
    public virtual ICollection<ViewHistory> ViewHistories { get; set; } = new List<ViewHistory>();
}
