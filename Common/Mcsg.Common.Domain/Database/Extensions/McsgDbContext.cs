namespace Mcsg.Common.Domain;

using Common.Domain.Interfaces;
using Domain.Entities;

/// <summary>
/// McsgContext
/// </summary>
partial class McsgContext : IMcsgContext
{
    #region -- Properties --

    #region -- IQueryable --
    public IQueryable<BackgroundMedia> BackgroundMediaAvailable => BackgroundMedias.Where(p => !p.IsDelete);

    public IQueryable<BackgroundMediaPost> BackgroundMediaPostAvailable => BackgroundMediaPosts.Where(p => !p.IsDelete);

    public IQueryable<ComicFollowedPost> ComicFollowedPostAvailable => ComicFollowedPosts.Where(p => !p.IsDelete);

    public IQueryable<ComicMetaData> ComicMetaDataAvailable => ComicMetaDatas;

    public IQueryable<ComicPost> ComicPostAvailable => ComicPosts.Where(p => !p.IsDelete);

    public IQueryable<ComicPostComment> ComicPostCommentAvailable => ComicPostComments.Where(p => !p.IsDelete);

    public IQueryable<ComicPostCommentReaction> ComicPostCommentReactionAvailable => ComicPostCommentReactions.Where(p => !p.IsDelete);

    public IQueryable<ComicPostFavorite> ComicPostFavoriteAvailable => ComicPostFavorites.Where(p => !p.IsDelete);

    public IQueryable<ComicPostLink> ComicPostLinkAvailable => ComicPostLinks.Where(p => !p.IsDelete);

    public IQueryable<ComicPostReaction> ComicPostReactionAvailable => ComicPostReactions.Where(p => !p.IsDelete);

    public IQueryable<ComicPostReport> ComicPostReportAvailable => ComicPostReports.Where(p => !p.IsDelete);

    public IQueryable<ComicResource> ComicResourceAvailable => ComicResources.Where(p => !p.IsDelete);

    public IQueryable<ComicSubPost> ComicSubPostAvailable => ComicSubPosts.Where(p => !p.IsDelete);

    public IQueryable<ComicSubPostComment> ComicSubPostCommentAvailable => ComicSubPostComments.Where(p => !p.IsDelete);

    public IQueryable<ComicSubPostCommentReaction> ComicSubPostCommentReactionAvailable => ComicSubPostCommentReactions.Where(p => !p.IsDelete);

    public IQueryable<ComicSubPostReaction> ComicSubPostReactionAvailable => ComicSubPostReactions.Where(p => !p.IsDelete);

    public IQueryable<ComicTagPost> ComicTagPostAvailable => ComicTagPosts.Where(p => !p.IsDelete);

    public IQueryable<CrawComic> CrawComicAvailable => CrawComics.Where(p => !p.IsDelete);

    public IQueryable<CrawComicChapter> CrawComicChapterAvailable => CrawComicChapters.Where(p => !p.IsDelete);

    public IQueryable<Job> JobAvailable => Jobs.Where(p => !p.IsDelete);

    public IQueryable<Mention> MentionAvailable => Mentions.Where(p => !p.IsDelete);

    public IQueryable<Notification> NotificationAvailable => Notifications.Where(p => !p.IsDelete);

    public IQueryable<NotificationObject> NotificationObjectAvailable => NotificationObjects.Where(p => !p.IsDelete);

    public IQueryable<Role> RoleAvailable => Roles;

    //public IQueryable<RoleClaim> RoleClaimAvailable => RoleClaims;

    public IQueryable<Session> SessionAvailable => Sessions.Where(p => !p.IsDelete);

    public IQueryable<SmartCountAction> SmartCountActionAvailable => SmartCountActions;

    public IQueryable<SmartLookup> SmartLookupAvailable => SmartLookups;

    public IQueryable<SmartLookupUser> SmartLookupUserAvailable => SmartLookupUsers;

    public IQueryable<SocialMetaData> SocialMetaDataAvailable => SocialMetaDatas;

    public IQueryable<SocialPost> SocialPostAvailable => SocialPosts.Where(p => !p.IsDelete);

    public IQueryable<SocialPostComment> SocialPostCommentAvailable => SocialPostComments.Where(p => !p.IsDelete);

    public IQueryable<SocialPostCommentReaction> SocialPostCommentReactionAvailable => SocialPostCommentReactions.Where(p => !p.IsDelete);

    public IQueryable<SocialPostFavorite> SocialPostFavoriteAvailable => SocialPostFavorites.Where(p => !p.IsDelete);

    public IQueryable<SocialPostLink> SocialPostLinkAvailable => SocialPostLinks.Where(p => !p.IsDelete);

    public IQueryable<SocialPostReaction> SocialPostReactionAvailable => SocialPostReactions.Where(p => !p.IsDelete);

    public IQueryable<SocialPostReport> SocialPostReportAvailable => SocialPostReports.Where(p => !p.IsDelete);

    public IQueryable<SocialResource> SocialResourceAvailable => SocialResources.Where(p => !p.IsDelete);

    public IQueryable<SocialSubPost> SocialSubPostAvailable => SocialSubPosts.Where(p => !p.IsDelete);

    public IQueryable<SocialSubPostComment> SocialSubPostCommentAvailable => SocialSubPostComments.Where(p => !p.IsDelete);

    public IQueryable<SocialSubPostCommentReaction> SocialSubPostCommentReactionAvailable => SocialSubPostCommentReactions.Where(p => !p.IsDelete);

    public IQueryable<SocialSubPostReaction> SocialSubPostReactionAvailable => SocialSubPostReactions.Where(p => !p.IsDelete);

    public IQueryable<SocialTagPost> SocialTagPostAvailable => SocialTagPosts.Where(p => !p.IsDelete);

    public IQueryable<StoryFollowedPost> StoryFollowedPostAvailable => StoryFollowedPosts.Where(p => !p.IsDelete);

    public IQueryable<StoryMetaData> StoryMetaDataAvailable => StoryMetaDatas;

    public IQueryable<StoryPost> StoryPostAvailable => StoryPosts.Where(p => !p.IsDelete);

    public IQueryable<StoryPostComment> StoryPostCommentAvailable => StoryPostComments.Where(p => !p.IsDelete);

    public IQueryable<StoryPostCommentReaction> StoryPostCommentReactionAvailable => StoryPostCommentReactions.Where(p => !p.IsDelete);

    public IQueryable<StoryPostFavorite> StoryPostFavoriteAvailable => StoryPostFavorites.Where(p => !p.IsDelete);

    public IQueryable<StoryPostLink> StoryPostLinkAvailable => StoryPostLinks.Where(p => !p.IsDelete);

    public IQueryable<StoryPostReaction> StoryPostReactionAvailable => StoryPostReactions.Where(p => !p.IsDelete);

    public IQueryable<StoryPostReport> StoryPostReportAvailable => StoryPostReports.Where(p => !p.IsDelete);

    public IQueryable<StoryResource> StoryResourceAvailable => StoryResources.Where(p => !p.IsDelete);

    public IQueryable<StorySubPost> StorySubPostAvailable => StorySubPosts.Where(p => !p.IsDelete);

    public IQueryable<StorySubPostComment> StorySubPostCommentAvailable => StorySubPostComments.Where(p => !p.IsDelete);

    public IQueryable<StorySubPostCommentReaction> StorySubPostCommentReactionAvailable => StorySubPostCommentReactions.Where(p => !p.IsDelete);

    public IQueryable<StorySubPostReaction> StorySubPostReactionAvailable => StorySubPostReactions.Where(p => !p.IsDelete);

    public IQueryable<StoryTagPost> StoryTagPostAvailable => StoryTagPosts.Where(p => !p.IsDelete);

    public IQueryable<SystemSetting> SystemSettingAvailable => SystemSettings.Where(p => !p.IsDelete);

    public IQueryable<SystemSettingHistory> SystemSettingHistoryAvailable => SystemSettingHistories.Where(p => !p.IsDelete);

    public IQueryable<Tag> TagAvailable => Tags.Where(p => !p.IsDelete);

    public IQueryable<TagFavorite> TagFavoriteAvailable => TagFavorites.Where(p => !p.IsDelete);

    public IQueryable<User> UserAvailable => Users.Where(p => !p.IsDelete);

    //public IQueryable<UserClaim> UserClaimAvailable => UserClaims;

    public IQueryable<UserExclusiveSubPost> UserExclusiveSubPostAvailable => UserExclusiveSubPosts.Where(p => !p.IsDelete);

    public IQueryable<UserFollow> UserFollowAvailable => UserFollows.Where(p => !p.IsDelete);

    //public IQueryable<UserLogin> UserLoginAvailable => UserLogins;

    public IQueryable<UserNameHistory> UserNameHistoryAvailable => UserNameHistories.Where(p => !p.IsDelete);

    public IQueryable<UserOtp> UserOtpAvailable => UserOtps.Where(p => !p.IsDelete);

    public IQueryable<UserRefreshToken> UserRefreshTokenAvailable => UserRefreshTokens.Where(p => !p.IsDelete);

    public IQueryable<UserSocial> UserSocialAvailable => UserSocials.Where(p => !p.IsDelete);

    //public IQueryable<UserToken> UserTokenAvailable => UserTokens;

    public IQueryable<ViewHistory> ViewHistoryAvailable => ViewHistories;
    #endregion

    #endregion
}
