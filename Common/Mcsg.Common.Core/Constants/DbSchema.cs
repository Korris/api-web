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
    /// Story
    /// </summary>
    public const string Story = "story";

    /// <summary>
    /// Comic tables
    /// </summary>
    public const string ComicTables = "ComicPostCommentReactions;ComicPostComments;ComicPostReactions;ComicPosts;ComicResources;ComicSubPostCommentReactions;ComicSubPostComments;ComicSubPostReactions;ComicSubPosts";

    /// <summary>
    /// Identity tables
    /// </summary>
    public const string IdentityTables = "Users;UserNameHistories;UserClaims;UserLogins;UserTokens;Roles;RoleClaims;UserRoles";

    /// <summary>
    /// Story tables
    /// </summary>
    public const string StoryTables = "StoryPostCommentReactions;StoryPostComments;StoryPostReactions;StoryPosts;StoryResources;StorySubPostCommentReactions;StorySubPostComments;StorySubPostReactions;StorySubPosts";

    /// <summary>
    /// SystemSettingHistories table
    /// </summary>
    public const string SystemSettingHistoriesTable = "SystemSettingHistories";
}