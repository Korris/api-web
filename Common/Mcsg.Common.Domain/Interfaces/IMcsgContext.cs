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

namespace Mcsg.Common.Domain.Interfaces;

using Domain.Entities;

/// <summary>
/// Interface Mcsg context
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

    #endregion

    #region -- Properties --

    /// <summary>
    /// Database
    /// </summary>
    DatabaseFacade Database { get; }

    #region -- DbSet --
    DbSet<BackgroundMedia> BackgroundMedias { get; set; }

    DbSet<BackgroundMediaPost> BackgroundMediaPosts { get; set; }

    DbSet<ComicFollowedPost> ComicFollowedPosts { get; set; }

    DbSet<ComicMetaData> ComicMetaDatas { get; set; }

    DbSet<ComicPost> ComicPosts { get; set; }

    DbSet<ComicPostComment> ComicPostComments { get; set; }

    DbSet<ComicPostCommentReaction> ComicPostCommentReactions { get; set; }

    DbSet<ComicPostFavorite> ComicPostFavorites { get; set; }

    DbSet<ComicPostLink> ComicPostLinks { get; set; }

    DbSet<ComicPostReaction> ComicPostReactions { get; set; }

    DbSet<ComicPostReport> ComicPostReports { get; set; }

    DbSet<ComicResource> ComicResources { get; set; }

    DbSet<ComicSubPost> ComicSubPosts { get; set; }

    DbSet<ComicSubPostComment> ComicSubPostComments { get; set; }

    DbSet<ComicSubPostCommentReaction> ComicSubPostCommentReactions { get; set; }

    DbSet<ComicSubPostReaction> ComicSubPostReactions { get; set; }

    DbSet<ComicTagPost> ComicTagPosts { get; set; }

    DbSet<CrawComic> CrawComics { get; set; }

    DbSet<CrawComicChapter> CrawComicChapters { get; set; }

    DbSet<Job> Jobs { get; set; }

    DbSet<Mention> Mentions { get; set; }

    DbSet<Notification> Notifications { get; set; }

    DbSet<NotificationObject> NotificationObjects { get; set; }

    DbSet<Role> Roles { get; set; }

    //DbSet<RoleClaim> RoleClaims { get; set; }

    DbSet<Session> Sessions { get; set; }

    DbSet<SmartCountAction> SmartCountActions { get; set; }

    DbSet<SmartLookup> SmartLookups { get; set; }

    DbSet<SmartLookupUser> SmartLookupUsers { get; set; }

    DbSet<SocialMetaData> SocialMetaDatas { get; set; }

    DbSet<SocialPost> SocialPosts { get; set; }

    DbSet<SocialPostComment> SocialPostComments { get; set; }

    DbSet<SocialPostCommentReaction> SocialPostCommentReactions { get; set; }

    DbSet<SocialPostFavorite> SocialPostFavorites { get; set; }

    DbSet<SocialPostLink> SocialPostLinks { get; set; }

    DbSet<SocialPostReaction> SocialPostReactions { get; set; }

    DbSet<SocialPostReport> SocialPostReports { get; set; }

    DbSet<SocialResource> SocialResources { get; set; }

    DbSet<SocialSubPost> SocialSubPosts { get; set; }

    DbSet<SocialSubPostComment> SocialSubPostComments { get; set; }

    DbSet<SocialSubPostCommentReaction> SocialSubPostCommentReactions { get; set; }

    DbSet<SocialSubPostReaction> SocialSubPostReactions { get; set; }

    DbSet<SocialTagPost> SocialTagPosts { get; set; }

    DbSet<StoryFollowedPost> StoryFollowedPosts { get; set; }

    DbSet<StoryMetaData> StoryMetaDatas { get; set; }

    DbSet<StoryPost> StoryPosts { get; set; }

    DbSet<StoryPostComment> StoryPostComments { get; set; }

    DbSet<StoryPostCommentReaction> StoryPostCommentReactions { get; set; }

    DbSet<StoryPostFavorite> StoryPostFavorites { get; set; }

    DbSet<StoryPostLink> StoryPostLinks { get; set; }

    DbSet<StoryPostReaction> StoryPostReactions { get; set; }

    DbSet<StoryPostReport> StoryPostReports { get; set; }

    DbSet<StoryResource> StoryResources { get; set; }

    DbSet<StorySubPost> StorySubPosts { get; set; }

    DbSet<StorySubPostComment> StorySubPostComments { get; set; }

    DbSet<StorySubPostCommentReaction> StorySubPostCommentReactions { get; set; }

    DbSet<StorySubPostReaction> StorySubPostReactions { get; set; }

    DbSet<StoryTagPost> StoryTagPosts { get; set; }

    DbSet<SystemSetting> SystemSettings { get; set; }

    DbSet<SystemSettingHistory> SystemSettingHistories { get; set; }

    DbSet<Tag> Tags { get; set; }

    DbSet<TagFavorite> TagFavorites { get; set; }

    DbSet<User> Users { get; set; }

    //DbSet<UserClaim> UserClaims { get; set; }

    DbSet<UserExclusiveSubPost> UserExclusiveSubPosts { get; set; }

    DbSet<UserFollow> UserFollows { get; set; }

    //DbSet<UserLogin> UserLogins { get; set; }

    DbSet<UserNameHistory> UserNameHistories { get; set; }

    DbSet<UserOtp> UserOtps { get; set; }

    DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    DbSet<UserSocial> UserSocials { get; set; }

    //DbSet<UserToken> UserTokens { get; set; }

    DbSet<ViewHistory> ViewHistories { get; set; }
    #endregion

    #region -- IQueryable --
    IQueryable<BackgroundMedia> BackgroundMediaAvailable { get; }

    IQueryable<BackgroundMediaPost> BackgroundMediaPostAvailable { get; }

    IQueryable<ComicFollowedPost> ComicFollowedPostAvailable { get; }

    IQueryable<ComicMetaData> ComicMetaDataAvailable { get; }

    IQueryable<ComicPost> ComicPostAvailable { get; }

    IQueryable<ComicPostComment> ComicPostCommentAvailable { get; }

    IQueryable<ComicPostCommentReaction> ComicPostCommentReactionAvailable { get; }

    IQueryable<ComicPostFavorite> ComicPostFavoriteAvailable { get; }

    IQueryable<ComicPostLink> ComicPostLinkAvailable { get; }

    IQueryable<ComicPostReaction> ComicPostReactionAvailable { get; }

    IQueryable<ComicPostReport> ComicPostReportAvailable { get; }

    IQueryable<ComicResource> ComicResourceAvailable { get; }

    IQueryable<ComicSubPost> ComicSubPostAvailable { get; }

    IQueryable<ComicSubPostComment> ComicSubPostCommentAvailable { get; }

    IQueryable<ComicSubPostCommentReaction> ComicSubPostCommentReactionAvailable { get; }

    IQueryable<ComicSubPostReaction> ComicSubPostReactionAvailable { get; }

    IQueryable<ComicTagPost> ComicTagPostAvailable { get; }

    IQueryable<CrawComic> CrawComicAvailable { get; }

    IQueryable<CrawComicChapter> CrawComicChapterAvailable { get; }

    IQueryable<Job> JobAvailable { get; }

    IQueryable<Mention> MentionAvailable { get; }

    IQueryable<Notification> NotificationAvailable { get; }

    IQueryable<NotificationObject> NotificationObjectAvailable { get; }

    IQueryable<Role> RoleAvailable { get; }

    //IQueryable<RoleClaim> RoleClaimAvailable { get; }

    IQueryable<Session> SessionAvailable { get; }

    IQueryable<SmartCountAction> SmartCountActionAvailable { get; }

    IQueryable<SmartLookup> SmartLookupAvailable { get; }

    IQueryable<SmartLookupUser> SmartLookupUserAvailable { get; }

    IQueryable<SocialMetaData> SocialMetaDataAvailable { get; }

    IQueryable<SocialPost> SocialPostAvailable { get; }

    IQueryable<SocialPostComment> SocialPostCommentAvailable { get; }

    IQueryable<SocialPostCommentReaction> SocialPostCommentReactionAvailable { get; }

    IQueryable<SocialPostFavorite> SocialPostFavoriteAvailable { get; }

    IQueryable<SocialPostLink> SocialPostLinkAvailable { get; }

    IQueryable<SocialPostReaction> SocialPostReactionAvailable { get; }

    IQueryable<SocialPostReport> SocialPostReportAvailable { get; }

    IQueryable<SocialResource> SocialResourceAvailable { get; }

    IQueryable<SocialSubPost> SocialSubPostAvailable { get; }

    IQueryable<SocialSubPostComment> SocialSubPostCommentAvailable { get; }

    IQueryable<SocialSubPostCommentReaction> SocialSubPostCommentReactionAvailable { get; }

    IQueryable<SocialSubPostReaction> SocialSubPostReactionAvailable { get; }

    IQueryable<SocialTagPost> SocialTagPostAvailable { get; }

    IQueryable<StoryFollowedPost> StoryFollowedPostAvailable { get; }

    IQueryable<StoryMetaData> StoryMetaDataAvailable { get; }

    IQueryable<StoryPost> StoryPostAvailable { get; }

    IQueryable<StoryPostComment> StoryPostCommentAvailable { get; }

    IQueryable<StoryPostCommentReaction> StoryPostCommentReactionAvailable { get; }

    IQueryable<StoryPostFavorite> StoryPostFavoriteAvailable { get; }

    IQueryable<StoryPostLink> StoryPostLinkAvailable { get; }

    IQueryable<StoryPostReaction> StoryPostReactionAvailable { get; }

    IQueryable<StoryPostReport> StoryPostReportAvailable { get; }

    IQueryable<StoryResource> StoryResourceAvailable { get; }

    IQueryable<StorySubPost> StorySubPostAvailable { get; }

    IQueryable<StorySubPostComment> StorySubPostCommentAvailable { get; }

    IQueryable<StorySubPostCommentReaction> StorySubPostCommentReactionAvailable { get; }

    IQueryable<StorySubPostReaction> StorySubPostReactionAvailable { get; }

    IQueryable<StoryTagPost> StoryTagPostAvailable { get; }

    IQueryable<SystemSetting> SystemSettingAvailable { get; }

    IQueryable<SystemSettingHistory> SystemSettingHistoryAvailable { get; }

    IQueryable<Tag> TagAvailable { get; }

    IQueryable<TagFavorite> TagFavoriteAvailable { get; }

    IQueryable<User> UserAvailable { get; }

    //IQueryable<UserClaim> UserClaimAvailable { get; }

    IQueryable<UserExclusiveSubPost> UserExclusiveSubPostAvailable { get; }

    IQueryable<UserFollow> UserFollowAvailable { get; }

    //IQueryable<UserLogin> UserLoginAvailable { get; }

    IQueryable<UserNameHistory> UserNameHistoryAvailable { get; }

    IQueryable<UserOtp> UserOtpAvailable { get; }

    IQueryable<UserRefreshToken> UserRefreshTokenAvailable { get; }

    IQueryable<UserSocial> UserSocialAvailable { get; }

    //IQueryable<UserToken> UserTokenAvailable { get; }

    IQueryable<ViewHistory> ViewHistoryAvailable { get; }
    #endregion

    #endregion
}
