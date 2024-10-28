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
    public const string ComicTables = "ComicMetaDatas;ComicPostCommentReactions;ComicPostComments;ComicPostFavorites;ComicPostHides;ComicPostLinks;ComicPostReactions;ComicPostReports;ComicPosts;ComicResources;ComicSubPostCommentReactions;ComicSubPostComments;ComicSubPostReactions;ComicSubPosts;ComicTagPosts";

    /// <summary>
    /// Document tables
    /// </summary>
    public const string DocumentTables = "DocumentMetaDatas;DocumentPostCommentReactions;DocumentPostComments;DocumentPostFavorites;DocumentPostHides;DocumentPostLinks;DocumentPostReactions;DocumentPostReports;DocumentPosts;DocumentResources;DocumentSubPostCommentReactions;DocumentSubPostComments;DocumentSubPostReactions;DocumentSubPosts;DocumentTagPosts";

    /// <summary>
    /// Identity tables
    /// </summary>
    public const string IdentityTables = "RoleClaims;Roles;Sessions;UserClaims;UserFollows;UserLogins;UserNameHistories;UserOtps;UserReferrals;UserRefreshTokens;UserRoles;Users;UserSocials;UserTokens";

    /// <summary>
    /// Social tables
    /// </summary>
    public const string SocialTables = "SocialMetaDatas;SocialPostCommentReactions;SocialPostComments;SocialPostFavorites;SocialPostHides;SocialPostLinks;SocialPostReactions;SocialPostReports;SocialPosts;SocialResources;SocialSubPostCommentReactions;SocialSubPostComments;SocialSubPostReactions;SocialSubPosts;SocialTagPosts";

    /// <summary>
    /// Story tables
    /// </summary>
    public const string StoryTables = "StoryMetaDatas;StoryPostCommentReactions;StoryPostComments;StoryPostFavorites;StoryPostHides;StoryPostLinks;StoryPostReactions;StoryPostReports;StoryPosts;StoryResources;StorySubPostCommentReactions;StorySubPostComments;StorySubPostReactions;StorySubPosts;StoryTagPosts";

    /// <summary>
    /// System tables
    /// </summary>
    public const string SystemTables = "Jobs;NotificationObjects;Notifications;SystemSettingHistories;SystemSettings";
}
