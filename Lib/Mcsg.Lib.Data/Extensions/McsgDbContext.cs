namespace Mcsg.Lib.Data;

using Domain.Entities;

/// <summary>
/// McsgDbContext
/// </summary>
partial class McsgDbContext
{
    #region -- Properties --

    /// <summary>
    /// UserAvailable
    /// </summary>
    public IQueryable<User> UserAvailable => Users.Where(p => !p.IsDelete);

    /// <summary>
    /// UserFollowAvailable
    /// </summary>
    public IQueryable<UserFollow> UserFollowAvailable => UserFollows.Where(p => !p.IsDelete);

    public IQueryable<Session> SessionAvailable => Sessions.Where(p => !p.IsDelete);
    public IQueryable<UserOtp> UserOtpAvailable => UserOtps.Where(p => !p.IsDelete);
    public IQueryable<UserNameHistory> UserNameHistoryAvailable => UserNameHistories.Where(p => !p.IsDelete);
    public IQueryable<UserSocial> UserSocialAvailable => UserSocials.Where(p => !p.IsDelete);
    public IQueryable<Job> JobAvailable => Jobs.Where(p => !p.IsDelete);
    public IQueryable<UserRefreshToken> UserRefreshTokenAvailable => UserRefreshTokens.Where(p => !p.IsDelete);
    public IQueryable<Post> PostAvailable => Posts.Where(p => !p.IsDelete);
    public IQueryable<SubPost> SubPostAvailable => SubPosts.Where(p => !p.IsDelete);
    public IQueryable<PostComment> PostCommentAvailable => PostComments.Where(p => !p.IsDelete);
    public IQueryable<PostReaction> PostReactionAvailable => PostReactions.Where(p => !p.IsDelete);
    public IQueryable<SubPostReaction> SubPostReactionAvailable => SubPostReactions.Where(p => !p.IsDelete);
    public IQueryable<PostCommentReaction> PostCommentReactionAvailable => PostCommentReactions.Where(p => !p.IsDelete);
    public IQueryable<SubPostCommentReaction> SubPostCommentReactionAvailable => SubPostCommentReactions.Where(p => !p.IsDelete);
    public IQueryable<MetaData> MetaDataAvailable => MetaDatas;
    public IQueryable<Resource> ResourceAvailable => Resources.Where(p => !p.IsDelete);
    public IQueryable<SystemSetting> SystemSettingAvailable => SystemSettings.Where(p => !p.IsDelete);
    public IQueryable<SystemSettingHistory> SystemSettingHistoryAvailable => SystemSettingHistories.Where(p => !p.IsDelete);
    public IQueryable<Tag> TagAvailable => Tags.Where(p => !p.IsDelete);
    public IQueryable<TagPost> TagPostAvailable => TagPosts.Where(p => !p.IsDelete);
    public IQueryable<TagFavorite> TagFavoriteAvailable => TagFavorites.Where(p => !p.IsDelete);
    public IQueryable<PostFavorite> PostFavoriteAvailable => PostFavorites.Where(p => !p.IsDelete);
    public IQueryable<SmartLookup> SmartLookupAvailable => SmartLookups;
    public IQueryable<SmartCountAction> SmartCountActionAvailable => SmartCountActions;
    public IQueryable<SmartLookupUser> SmartLookupUserAvailable => SmartLookupUsers;
    public IQueryable<Notification> NotificationAvailable => Notifications.Where(p => !p.IsDelete);
    public IQueryable<NotificationObject> NotificationObjectAvailable => NotificationObjects.Where(p => !p.IsDelete);
    public IQueryable<BackgroundMedia> BackgroundMediaAvailable => BackgroundMedias.Where(p => !p.IsDelete);
    public IQueryable<BackgroundMediaPost> BackgroundMediaPostAvailable => BackgroundMediaPosts.Where(p => !p.IsDelete);
    public IQueryable<Mention> MentionAvailable => Mentions.Where(p => !p.IsDelete);
    public IQueryable<CrawComic> CrawComicAvailable => CrawComics.Where(p => !p.IsDelete);
    public IQueryable<CrawComicChapter> CrawComicChapterAvailable => CrawComicChapters.Where(p => !p.IsDelete);
    public IQueryable<PostReport> PostReportAvailable => PostReports.Where(p => !p.IsDelete);
    public IQueryable<PostLink> PostLinkAvailable => PostLinks.Where(p => !p.IsDelete);

    #endregion
}
