namespace Mcsg.Lib.Data.Constants;

public class DbSchema
{
    public const string Comic = "comic";
    public const string Default = "public.";
    public const string Identity = "identity";
    public const string Story = "story";
    public const string ComicTables = "ComicPostCommentReactions;ComicPostComments;ComicPostReactions;ComicPosts;ComicResources;ComicSubPostCommentReactions;ComicSubPostComments;ComicSubPostReactions;ComicSubPosts";
    public const string IdentityTables = "Users;UserNameHistories;UserClaims;UserLogins;UserTokens;Roles;RoleClaims;UserRoles";
    public const string StoryTables = "StoryPostCommentReactions;StoryPostComments;StoryPostReactions;StoryPosts;StoryResources;StorySubPostCommentReactions;StorySubPostComments;StorySubPostReactions;StorySubPosts";
    public const string SystemSettingHistories_Table = "SystemSettingHistories";
}