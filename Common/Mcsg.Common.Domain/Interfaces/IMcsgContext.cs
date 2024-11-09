#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-18 08:13
 * Update       : 2024-Jan-18 08:13
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenIddict.EntityFrameworkCore.Models;

namespace Mcsg.Common.Domain;

using Entities;

/// <summary>
/// Interface McsgContext
/// </summary>
public interface IMcsgContext
{
    #region -- Methods --

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database</returns>
    int SaveChanges();

    /// <summary>
    /// Make serial number
    /// </summary>
    /// <param name="q">Queryable</param>
    /// <param name="sOrderBy">Selector for OrderBy statement</param>
    /// <param name="sSelect">Selector for Select statement</param>
    /// <param name="prefix">Prefix</param>
    /// <param name="useDateTime">Use DateTime</param>
    /// <returns>Return the result</returns>
    string MakeNo<T>(IQueryable<T> q, Func<T, ulong> sOrderBy, Func<T, string> sSelect, string prefix, bool useDateTime = false);

    #endregion

    #region -- Properties --

    /// <summary>
    /// Database
    /// </summary>
    DatabaseFacade Database { get; }

    #region -- DbSet --
    DbSet<BackgroundMedia> BackgroundMedias { get; set; }

    DbSet<BackgroundMediaPost> BackgroundMediaPosts { get; set; }

    DbSet<ComicMetaData> ComicMetaDatas { get; set; }

    DbSet<ComicPost> ComicPosts { get; set; }

    DbSet<ComicPostComment> ComicPostComments { get; set; }

    DbSet<ComicPostCommentReaction> ComicPostCommentReactions { get; set; }

    DbSet<ComicPostFavorite> ComicPostFavorites { get; set; }

    DbSet<ComicPostHide> ComicPostHides { get; set; }

    DbSet<ComicPostLink> ComicPostLinks { get; set; }

    DbSet<ComicPostReaction> ComicPostReactions { get; set; }

    DbSet<ComicPostShare> ComicPostShares { get; set; }

    DbSet<ComicReport> ComicReports { get; set; }

    DbSet<ComicReportDetail> ComicReportDetails { get; set; }

    DbSet<ComicResource> ComicResources { get; set; }

    DbSet<ComicSubPost> ComicSubPosts { get; set; }

    DbSet<ComicSubPostComment> ComicSubPostComments { get; set; }

    DbSet<ComicSubPostCommentReaction> ComicSubPostCommentReactions { get; set; }

    DbSet<ComicSubPostReaction> ComicSubPostReactions { get; set; }

    DbSet<ComicTagPost> ComicTagPosts { get; set; }

    DbSet<CrawComic> CrawComics { get; set; }

    DbSet<CrawComicChapter> CrawComicChapters { get; set; }

    DbSet<Device> Devices { get; set; }

    DbSet<DocumentMetaData> DocumentMetaDatas { get; set; }

    DbSet<DocumentPost> DocumentPosts { get; set; }

    DbSet<DocumentPostComment> DocumentPostComments { get; set; }

    DbSet<DocumentPostCommentReaction> DocumentPostCommentReactions { get; set; }

    DbSet<DocumentPostFavorite> DocumentPostFavorites { get; set; }

    DbSet<DocumentPostHide> DocumentPostHides { get; set; }

    DbSet<DocumentPostLink> DocumentPostLinks { get; set; }

    DbSet<DocumentPostReaction> DocumentPostReactions { get; set; }

    DbSet<DocumentPostShare> DocumentPostShares { get; set; }

    DbSet<DocumentReport> DocumentReports { get; set; }

    DbSet<DocumentReportDetail> DocumentReportDetails { get; set; }

    DbSet<DocumentResource> DocumentResources { get; set; }

    DbSet<DocumentSubPost> DocumentSubPosts { get; set; }

    DbSet<DocumentSubPostComment> DocumentSubPostComments { get; set; }

    DbSet<DocumentSubPostCommentReaction> DocumentSubPostCommentReactions { get; set; }

    DbSet<DocumentSubPostReaction> DocumentSubPostReactions { get; set; }

    DbSet<DocumentTagPost> DocumentTagPosts { get; set; }

    DbSet<Feedback> Feedbacks { get; set; }

    DbSet<Job> Jobs { get; set; }

    DbSet<Mention> Mentions { get; set; }

    DbSet<Notification> Notifications { get; set; }

    DbSet<NotificationObject> NotificationObjects { get; set; }

    DbSet<OpenIddictEntityFrameworkCoreApplication> OpenIdApplications { get; set; }

    DbSet<OpenIddictEntityFrameworkCoreAuthorization> OpenIdAuthorizations { get; set; }

    DbSet<OpenIddictEntityFrameworkCoreScope> OpenIdScopes { get; set; }

    DbSet<OpenIddictEntityFrameworkCoreToken> OpenIdTokens { get; set; }

    DbSet<Rating> Ratings { get; set; }

    DbSet<Role> Roles { get; set; }

    //DbSet<RoleClaim> RoleClaims { get; set; }

    DbSet<SmartCountAction> SmartCountActions { get; set; }

    DbSet<SmartLookup> SmartLookups { get; set; }

    DbSet<SmartLookupUser> SmartLookupUsers { get; set; }

    DbSet<SocialMetaData> SocialMetaDatas { get; set; }

    DbSet<SocialPost> SocialPosts { get; set; }

    DbSet<SocialPostComment> SocialPostComments { get; set; }

    DbSet<SocialPostCommentReaction> SocialPostCommentReactions { get; set; }

    DbSet<SocialPostFavorite> SocialPostFavorites { get; set; }

    DbSet<SocialPostHide> SocialPostHides { get; set; }

    DbSet<SocialPostLink> SocialPostLinks { get; set; }

    DbSet<SocialPostReaction> SocialPostReactions { get; set; }

    DbSet<SocialPostShare> SocialPostShares { get; set; }

    DbSet<SocialReport> SocialReports { get; set; }

    DbSet<SocialReportDetail> SocialReportDetails { get; set; }

    DbSet<SocialResource> SocialResources { get; set; }

    DbSet<SocialSubPost> SocialSubPosts { get; set; }

    DbSet<SocialSubPostComment> SocialSubPostComments { get; set; }

    DbSet<SocialSubPostCommentReaction> SocialSubPostCommentReactions { get; set; }

    DbSet<SocialSubPostReaction> SocialSubPostReactions { get; set; }

    DbSet<SocialTagPost> SocialTagPosts { get; set; }

    DbSet<StoryMetaData> StoryMetaDatas { get; set; }

    DbSet<StoryPost> StoryPosts { get; set; }

    DbSet<StoryPostComment> StoryPostComments { get; set; }

    DbSet<StoryPostCommentReaction> StoryPostCommentReactions { get; set; }

    DbSet<StoryPostFavorite> StoryPostFavorites { get; set; }

    DbSet<StoryPostHide> StoryPostHides { get; set; }

    DbSet<StoryPostLink> StoryPostLinks { get; set; }

    DbSet<StoryPostReaction> StoryPostReactions { get; set; }

    DbSet<StoryPostShare> StoryPostShares { get; set; }

    DbSet<StoryReport> StoryReports { get; set; }

    DbSet<StoryReportDetail> StoryReportDetails { get; set; }

    DbSet<StoryResource> StoryResources { get; set; }

    DbSet<StorySubPost> StorySubPosts { get; set; }

    DbSet<StorySubPostComment> StorySubPostComments { get; set; }

    DbSet<StorySubPostCommentReaction> StorySubPostCommentReactions { get; set; }

    DbSet<StorySubPostReaction> StorySubPostReactions { get; set; }

    DbSet<StoryTagPost> StoryTagPosts { get; set; }

    DbSet<SystemResource> SystemResources { get; set; }

    DbSet<SystemSetting> SystemSettings { get; set; }

    DbSet<SystemSettingHistory> SystemSettingHistories { get; set; }

    DbSet<Tag> Tags { get; set; }

    DbSet<TagFavorite> TagFavorites { get; set; }

    DbSet<User> Users { get; set; }

    DbSet<UserBlock> UserBlocks { get; set; }

    //DbSet<UserClaim> UserClaims { get; set; }

    DbSet<UserExclusiveSubPost> UserExclusiveSubPosts { get; set; }

    DbSet<UserFollow> UserFollows { get; set; }

    //DbSet<UserLogin> UserLogins { get; set; }

    DbSet<UserNameHistory> UserNameHistories { get; set; }

    DbSet<UserOtp> UserOtps { get; set; }

    DbSet<UserReferral> UserReferrals { get; set; }

    DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    DbSet<UserRelation> UserRelations { get; set; }

    DbSet<UserSocial> UserSocials { get; set; }

    //DbSet<UserToken> UserTokens { get; set; }

    DbSet<ViewHistory> ViewHistories { get; set; }
    #endregion

    #region -- IQueryable --
    IQueryable<BackgroundMedia> BackgroundMediaAvailable { get; }

    IQueryable<BackgroundMediaPost> BackgroundMediaPostAvailable { get; }

    IQueryable<ComicMetaData> ComicMetaDataAvailable { get; }

    IQueryable<ComicPost> ComicPostAvailable { get; }

    IQueryable<ComicPostComment> ComicPostCommentAvailable { get; }

    IQueryable<ComicPostCommentReaction> ComicPostCommentReactionAvailable { get; }

    IQueryable<ComicPostFavorite> ComicPostFavoriteAvailable { get; }

    IQueryable<ComicPostHide> ComicPostHideAvailable { get; }

    IQueryable<ComicPostLink> ComicPostLinkAvailable { get; }

    IQueryable<ComicPostReaction> ComicPostReactionAvailable { get; }

    IQueryable<ComicPostShare> ComicPostShareAvailable { get; }

    IQueryable<ComicReport> ComicReportAvailable { get; }

    IQueryable<ComicReportDetail> ComicReportDetailAvailable { get; }

    IQueryable<ComicResource> ComicResourceAvailable { get; }

    IQueryable<ComicSubPost> ComicSubPostAvailable { get; }

    IQueryable<ComicSubPostComment> ComicSubPostCommentAvailable { get; }

    IQueryable<ComicSubPostCommentReaction> ComicSubPostCommentReactionAvailable { get; }

    IQueryable<ComicSubPostReaction> ComicSubPostReactionAvailable { get; }

    IQueryable<ComicTagPost> ComicTagPostAvailable { get; }

    IQueryable<CrawComic> CrawComicAvailable { get; }

    IQueryable<CrawComicChapter> CrawComicChapterAvailable { get; }

    IQueryable<Device> DeviceAvailable { get; }

    IQueryable<DocumentMetaData> DocumentMetaDataAvailable { get; }

    IQueryable<DocumentPost> DocumentPostAvailable { get; }

    IQueryable<DocumentPostComment> DocumentPostCommentAvailable { get; }

    IQueryable<DocumentPostCommentReaction> DocumentPostCommentReactionAvailable { get; }

    IQueryable<DocumentPostFavorite> DocumentPostFavoriteAvailable { get; }

    IQueryable<DocumentPostHide> DocumentPostHideAvailable { get; }

    IQueryable<DocumentPostLink> DocumentPostLinkAvailable { get; }

    IQueryable<DocumentPostReaction> DocumentPostReactionAvailable { get; }

    IQueryable<DocumentPostShare> DocumentPostShareAvailable { get; }

    IQueryable<DocumentReport> DocumentReportAvailable { get; }

    IQueryable<DocumentReportDetail> DocumentReportDetailAvailable { get; }

    IQueryable<DocumentResource> DocumentResourceAvailable { get; }

    IQueryable<DocumentSubPost> DocumentSubPostAvailable { get; }

    IQueryable<DocumentSubPostComment> DocumentSubPostCommentAvailable { get; }

    IQueryable<DocumentSubPostCommentReaction> DocumentSubPostCommentReactionAvailable { get; }

    IQueryable<DocumentSubPostReaction> DocumentSubPostReactionAvailable { get; }

    IQueryable<DocumentTagPost> DocumentTagPostAvailable { get; }

    IQueryable<Feedback> FeedbackAvailable { get; }

    IQueryable<Job> JobAvailable { get; }

    IQueryable<Mention> MentionAvailable { get; }

    IQueryable<Notification> NotificationAvailable { get; }

    IQueryable<NotificationObject> NotificationObjectAvailable { get; }

    IQueryable<OpenIddictEntityFrameworkCoreApplication> OpenIdApplicationAvailable { get; }

    IQueryable<OpenIddictEntityFrameworkCoreAuthorization> OpenIdAuthorizationAvailable { get; }

    IQueryable<OpenIddictEntityFrameworkCoreScope> OpenIdScopeAvailable { get; }

    IQueryable<OpenIddictEntityFrameworkCoreToken> OpenIdTokenAvailable { get; }

    IQueryable<Rating> RatingAvailable { get; }

    IQueryable<Role> RoleAvailable { get; }

    //IQueryable<RoleClaim> RoleClaimAvailable { get; }

    IQueryable<SmartCountAction> SmartCountActionAvailable { get; }

    IQueryable<SmartLookup> SmartLookupAvailable { get; }

    IQueryable<SmartLookupUser> SmartLookupUserAvailable { get; }

    IQueryable<SocialMetaData> SocialMetaDataAvailable { get; }

    IQueryable<SocialPost> SocialPostAvailable { get; }

    IQueryable<SocialPostComment> SocialPostCommentAvailable { get; }

    IQueryable<SocialPostCommentReaction> SocialPostCommentReactionAvailable { get; }

    IQueryable<SocialPostFavorite> SocialPostFavoriteAvailable { get; }

    IQueryable<SocialPostHide> SocialPostHideAvailable { get; }

    IQueryable<SocialPostLink> SocialPostLinkAvailable { get; }

    IQueryable<SocialPostReaction> SocialPostReactionAvailable { get; }

    IQueryable<SocialPostShare> SocialPostShareAvailable { get; }

    IQueryable<SocialReport> SocialReportAvailable { get; }

    IQueryable<SocialReportDetail> SocialReportDetailAvailable { get; }

    IQueryable<SocialResource> SocialResourceAvailable { get; }

    IQueryable<SocialSubPost> SocialSubPostAvailable { get; }

    IQueryable<SocialSubPostComment> SocialSubPostCommentAvailable { get; }

    IQueryable<SocialSubPostCommentReaction> SocialSubPostCommentReactionAvailable { get; }

    IQueryable<SocialSubPostReaction> SocialSubPostReactionAvailable { get; }

    IQueryable<SocialTagPost> SocialTagPostAvailable { get; }

    IQueryable<StoryMetaData> StoryMetaDataAvailable { get; }

    IQueryable<StoryPost> StoryPostAvailable { get; }

    IQueryable<StoryPostComment> StoryPostCommentAvailable { get; }

    IQueryable<StoryPostCommentReaction> StoryPostCommentReactionAvailable { get; }

    IQueryable<StoryPostFavorite> StoryPostFavoriteAvailable { get; }

    IQueryable<StoryPostHide> StoryPostHideAvailable { get; }

    IQueryable<StoryPostLink> StoryPostLinkAvailable { get; }

    IQueryable<StoryPostReaction> StoryPostReactionAvailable { get; }

    IQueryable<StoryPostShare> StoryPostShareAvailable { get; }

    IQueryable<StoryReport> StoryReportAvailable { get; }

    IQueryable<StoryReportDetail> StoryReportDetailAvailable { get; }

    IQueryable<StoryResource> StoryResourceAvailable { get; }

    IQueryable<StorySubPost> StorySubPostAvailable { get; }

    IQueryable<StorySubPostComment> StorySubPostCommentAvailable { get; }

    IQueryable<StorySubPostCommentReaction> StorySubPostCommentReactionAvailable { get; }

    IQueryable<StorySubPostReaction> StorySubPostReactionAvailable { get; }

    IQueryable<StoryTagPost> StoryTagPostAvailable { get; }

    IQueryable<SystemResource> SystemResourceAvailable { get; }

    IQueryable<SystemSetting> SystemSettingAvailable { get; }

    IQueryable<SystemSettingHistory> SystemSettingHistoryAvailable { get; }

    IQueryable<Tag> TagAvailable { get; }

    IQueryable<TagFavorite> TagFavoriteAvailable { get; }

    IQueryable<User> UserAvailable { get; }

    IQueryable<UserBlock> UserBlockAvailable { get; }

    //IQueryable<UserClaim> UserClaimAvailable { get; }

    IQueryable<UserExclusiveSubPost> UserExclusiveSubPostAvailable { get; }

    IQueryable<UserFollow> UserFollowAvailable { get; }

    //IQueryable<UserLogin> UserLoginAvailable { get; }

    IQueryable<UserNameHistory> UserNameHistoryAvailable { get; }

    IQueryable<UserOtp> UserOtpAvailable { get; }

    IQueryable<UserReferral> UserReferralAvailable { get; }

    IQueryable<UserRefreshToken> UserRefreshTokenAvailable { get; }

    IQueryable<UserRelation> UserRelationAvailable { get; }

    IQueryable<UserSocial> UserSocialAvailable { get; }

    //IQueryable<UserToken> UserTokenAvailable { get; }

    IQueryable<ViewHistory> ViewHistoryAvailable { get; }
    #endregion

    #endregion
}
