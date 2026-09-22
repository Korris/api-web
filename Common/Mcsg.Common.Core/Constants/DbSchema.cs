namespace Mcsg.Common.Core.Constants;

/// <summary>
/// DB schema
/// </summary>
public class DbSchema
{
    /// <summary>
    /// Comic
    /// </summary>
    public const string Comic = "comic";

    /// <summary>
    /// Document
    /// </summary>
    public const string Document = "document";

    /// <summary>
    /// Game
    /// </summary>
    public const string Game = "game";
    /// <summary>
    /// TapShow
    /// </summary>
    public const string TapShow = "tapshow";

    /// <summary>
    /// Default
    /// </summary>
    public const string Default = "public.";

    /// <summary>
    /// Identity
    /// </summary>
    public const string Identity = "identity";

    /// <summary>
    /// OpenId
    /// </summary>
    public const string OpenId = "openid";

    /// <summary>
    /// Social
    /// </summary>
    public const string Social = "social";

    /// <summary>
    /// Story
    /// </summary>
    public const string Story = "story";

    /// <summary>
    /// System
    /// </summary>
    public const string System = "system";

    /// <summary>
    /// Comic tables
    /// </summary>
    public const string ComicTables = "ComicMetaDatas;ComicPostCommentReactions;ComicPostComments;ComicPostFavorites;ComicPostHides;ComicPostLinks;ComicPostReactions;ComicPostReports;ComicReports;ComicReportDetails;ComicPosts;ComicResources;ComicSubPostCommentReactions;ComicSubPostComments;ComicSubPostReactions;ComicSubPosts;ComicTagPosts";

    /// <summary>
    /// Document tables
    /// </summary>
    public const string DocumentTables = "DocumentMetaDatas;DocumentPostCommentReactions;DocumentPostComments;DocumentPostFavorites;DocumentPostHides;DocumentPostLinks;DocumentPostReactions;DocumentPostReports;DocumentReports;DocumentReportDetails;DocumentPosts;DocumentResources;DocumentSubPostCommentReactions;DocumentSubPostComments;DocumentSubPostReactions;DocumentSubPosts;DocumentTagPosts";

    /// <summary>
    /// Game tables
    /// </summary>
    public const string GameTables = "GamePosts";
    /// <summary>
    /// TapShow tables
    /// </summary>
    public const string TapShowTables = "TapShowPosts;TapShowChapters;TapShowSegments;TapShowSegmentChoices;TapShowResources;TapShowPostComments;TapShowPostReactions;TapShowPostCommentReactions";

    /// <summary>
    /// Identity tables
    /// </summary>
    public const string IdentityTables = "RoleClaims;Roles;Sessions;UserClaims;UserFollows;UserLogins;UserNameHistories;UserOtps;UserReferrals;UserRefreshTokens;UserRoles;Users;UserSocials;UserTokens";

    /// <summary>
    /// Social tables
    /// </summary>
    public const string SocialTables = "SocialMetaDatas;SocialPostCommentReactions;SocialPostComments;SocialPostFavorites;SocialPostHides;SocialPostLinks;SocialPostReactions;SocialPostReports;SocialReports;SocialReportDetails;SocialPosts;SocialResources;SocialSubPostCommentReactions;SocialSubPostComments;SocialSubPostReactions;SocialSubPosts;SocialTagPosts";

    /// <summary>
    /// Story tables
    /// </summary>
    public const string StoryTables = "StoryMetaDatas;StoryPostCommentReactions;StoryPostComments;StoryPostFavorites;StoryPostHides;StoryPostLinks;StoryPostReactions;StoryPostReports;StoryReports;StoryReportDetails;;StoryPosts;StoryResources;StorySubPostCommentReactions;StorySubPostComments;StorySubPostReactions;StorySubPosts;StoryTagPosts";

    /// <summary>
    /// System tables
    /// </summary>
    public const string SystemTables = "Jobs;NotificationObjects;Notifications;SystemSettingHistories;SystemSettings";
}
