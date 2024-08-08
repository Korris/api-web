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
    /// Default
    /// </summary>
    public const string Default = "public.";

    /// <summary>
    /// Identity
    /// </summary>
    public const string Identity = "identity";

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
    public const string ComicTables = "ComicFollowedPosts;ComicMetaDatas;ComicPostCommentReactions;ComicPostComments;ComicPostFavorites;ComicPostHides;ComicPostLinks;ComicPostReactions;ComicPostReports;ComicPosts;ComicResources;ComicSubPostCommentReactions;ComicSubPostComments;ComicSubPostReactions;ComicSubPosts;ComicTagPosts";

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
    public const string StoryTables = "StoryFollowedPosts;StoryMetaDatas;StoryPostCommentReactions;StoryPostComments;StoryPostFavorites;StoryPostHides;StoryPostLinks;StoryPostReactions;StoryPostReports;StoryPosts;StoryResources;StorySubPostCommentReactions;StorySubPostComments;StorySubPostReactions;StorySubPosts;StoryTagPosts";

    /// <summary>
    /// System tables
    /// </summary>
    public const string SystemTables = "Jobs;NotificationObjects;Notifications;SystemSettingHistories;SystemSettings";
}