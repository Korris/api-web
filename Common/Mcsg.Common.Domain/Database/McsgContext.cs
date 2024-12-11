using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace Mcsg.Common.Domain;

using Core.Constants;
using Entities;

/// <summary>
/// McsgContext
/// </summary>
public partial class McsgContext : IdentityDbContext<User, Role, Guid>, IMcsgContext
{
    #region -- Overrides --

    /// <summary>
    /// On model creating
    /// </summary>
    /// <param name="builder">Builder</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(McsgContext).Assembly);

        builder.Entity<User>(p =>
        {
            p.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            p.ToTable("Users", DbSchema.Identity);
            p.Property(e => e.FirstName).IsRequired(false).HasMaxLength(256);
            p.Property(e => e.LastName).IsRequired(false).HasMaxLength(256);
            p.Property(e => e.Avatar).IsRequired(false).HasMaxLength(500);
            p.Property(e => e.ProfileName).IsRequired(false).HasMaxLength(256);
            p.HasIndex(x => x.ReferralCode);
        });

        builder.Entity<IdentityUserClaim<Guid>>(p => { p.ToTable("UserClaims", DbSchema.Identity); });
        builder.Entity<IdentityUserLogin<Guid>>(p => { p.ToTable("UserLogins", DbSchema.Identity); });
        builder.Entity<IdentityUserToken<Guid>>(p => p.ToTable("UserTokens", DbSchema.Identity));

        builder.Entity<Role>(p =>
        {
            p.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            p.ToTable("Roles", DbSchema.Identity);
        });

        builder.Entity<IdentityRoleClaim<Guid>>(p => { p.ToTable("RoleClaims", DbSchema.Identity); });
        builder.Entity<IdentityUserRole<Guid>>(p => p.ToTable("UserRoles", DbSchema.Identity));

        builder.Entity<OpenIddictEntityFrameworkCoreApplication>(p => p.ToTable("OpenIdApplications", DbSchema.OpenId));
        builder.Entity<OpenIddictEntityFrameworkCoreAuthorization>(p => p.ToTable("OpenIdAuthorizations", DbSchema.OpenId));
        builder.Entity<OpenIddictEntityFrameworkCoreScope>(p => p.ToTable("OpenIdScopes", DbSchema.OpenId));
        builder.Entity<OpenIddictEntityFrameworkCoreToken>(p => p.ToTable("OpenIdTokens", DbSchema.OpenId));

        //DataSeeder.Seed(builder);
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public McsgContext() { }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="options">Options</param>
    public McsgContext(DbContextOptions<McsgContext> options) : base(options) { }

    #endregion

    #region -- Properties --

    public virtual DbSet<BackgroundMedia> BackgroundMedias { get; set; }

    public virtual DbSet<BackgroundMediaPost> BackgroundMediaPosts { get; set; }

    public virtual DbSet<ComicMetaData> ComicMetaDatas { get; set; }

    public virtual DbSet<ComicPost> ComicPosts { get; set; }

    public virtual DbSet<ComicPostComment> ComicPostComments { get; set; }

    public virtual DbSet<ComicPostCommentReaction> ComicPostCommentReactions { get; set; }

    public virtual DbSet<ComicPostFavorite> ComicPostFavorites { get; set; }

    public virtual DbSet<ComicPostHide> ComicPostHides { get; set; }

    public virtual DbSet<ComicPostLink> ComicPostLinks { get; set; }

    public virtual DbSet<ComicPostReaction> ComicPostReactions { get; set; }

    public virtual DbSet<ComicPostShare> ComicPostShares { get; set; }

    public virtual DbSet<ComicReport> ComicReports { get; set; }

    public virtual DbSet<ComicReportDetail> ComicReportDetails { get; set; }

    public virtual DbSet<ComicResource> ComicResources { get; set; }

    public virtual DbSet<ComicSubPost> ComicSubPosts { get; set; }

    public virtual DbSet<ComicSubPostComment> ComicSubPostComments { get; set; }

    public virtual DbSet<ComicSubPostCommentReaction> ComicSubPostCommentReactions { get; set; }

    public virtual DbSet<ComicSubPostReaction> ComicSubPostReactions { get; set; }

    public virtual DbSet<ComicTagPost> ComicTagPosts { get; set; }

    public virtual DbSet<CrawComic> CrawComics { get; set; }

    public virtual DbSet<CrawComicChapter> CrawComicChapters { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<DocumentMetaData> DocumentMetaDatas { get; set; }

    public virtual DbSet<DocumentPost> DocumentPosts { get; set; }

    public virtual DbSet<DocumentPostComment> DocumentPostComments { get; set; }

    public virtual DbSet<DocumentPostCommentReaction> DocumentPostCommentReactions { get; set; }

    public virtual DbSet<DocumentPostFavorite> DocumentPostFavorites { get; set; }

    public virtual DbSet<DocumentPostHide> DocumentPostHides { get; set; }

    public virtual DbSet<DocumentPostLink> DocumentPostLinks { get; set; }

    public virtual DbSet<DocumentPostReaction> DocumentPostReactions { get; set; }

    public virtual DbSet<DocumentPostShare> DocumentPostShares { get; set; }

    public virtual DbSet<DocumentReport> DocumentReports { get; set; }

    public virtual DbSet<DocumentReportDetail> DocumentReportDetails { get; set; }

    public virtual DbSet<DocumentResource> DocumentResources { get; set; }

    public virtual DbSet<DocumentSubPost> DocumentSubPosts { get; set; }

    public virtual DbSet<DocumentSubPostComment> DocumentSubPostComments { get; set; }

    public virtual DbSet<DocumentSubPostCommentReaction> DocumentSubPostCommentReactions { get; set; }

    public virtual DbSet<DocumentSubPostReaction> DocumentSubPostReactions { get; set; }

    public virtual DbSet<DocumentTagPost> DocumentTagPosts { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<Mention> Mentions { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationObject> NotificationObjects { get; set; }

    public virtual DbSet<OpenIddictEntityFrameworkCoreApplication> OpenIdApplications { get; set; }

    public virtual DbSet<OpenIddictEntityFrameworkCoreAuthorization> OpenIdAuthorizations { get; set; }

    public virtual DbSet<OpenIddictEntityFrameworkCoreScope> OpenIdScopes { get; set; }

    public virtual DbSet<OpenIddictEntityFrameworkCoreToken> OpenIdTokens { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    //public virtual DbSet<Role> Roles { get; set; }

    //public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<SmartCountAction> SmartCountActions { get; set; }

    public virtual DbSet<SmartLookup> SmartLookups { get; set; }

    public virtual DbSet<SmartLookupUser> SmartLookupUsers { get; set; }

    public virtual DbSet<SocialMetaData> SocialMetaDatas { get; set; }

    public virtual DbSet<SocialPost> SocialPosts { get; set; }

    public virtual DbSet<SocialPostComment> SocialPostComments { get; set; }

    public virtual DbSet<SocialPostCommentReaction> SocialPostCommentReactions { get; set; }

    public virtual DbSet<SocialPostFavorite> SocialPostFavorites { get; set; }

    public virtual DbSet<SocialPostHide> SocialPostHides { get; set; }

    public virtual DbSet<SocialPostLink> SocialPostLinks { get; set; }

    public virtual DbSet<SocialPostReaction> SocialPostReactions { get; set; }

    public virtual DbSet<SocialPostShare> SocialPostShares { get; set; }

    public virtual DbSet<SocialReport> SocialReports { get; set; }

    public virtual DbSet<SocialReportDetail> SocialReportDetails { get; set; }

    public virtual DbSet<SocialResource> SocialResources { get; set; }

    public virtual DbSet<SocialSubPost> SocialSubPosts { get; set; }

    public virtual DbSet<SocialSubPostComment> SocialSubPostComments { get; set; }

    public virtual DbSet<SocialSubPostCommentReaction> SocialSubPostCommentReactions { get; set; }

    public virtual DbSet<SocialSubPostReaction> SocialSubPostReactions { get; set; }

    public virtual DbSet<SocialTagPost> SocialTagPosts { get; set; }

    public virtual DbSet<StoryMetaData> StoryMetaDatas { get; set; }

    public virtual DbSet<StoryPost> StoryPosts { get; set; }

    public virtual DbSet<StoryPostComment> StoryPostComments { get; set; }

    public virtual DbSet<StoryPostCommentReaction> StoryPostCommentReactions { get; set; }

    public virtual DbSet<StoryPostFavorite> StoryPostFavorites { get; set; }

    public virtual DbSet<StoryPostHide> StoryPostHides { get; set; }

    public virtual DbSet<StoryPostLink> StoryPostLinks { get; set; }

    public virtual DbSet<StoryPostReaction> StoryPostReactions { get; set; }

    public virtual DbSet<StoryPostShare> StoryPostShares { get; set; }

    public virtual DbSet<StoryReport> StoryReports { get; set; }

    public virtual DbSet<StoryReportDetail> StoryReportDetails { get; set; }

    public virtual DbSet<StoryResource> StoryResources { get; set; }

    public virtual DbSet<StorySubPost> StorySubPosts { get; set; }

    public virtual DbSet<StorySubPostComment> StorySubPostComments { get; set; }

    public virtual DbSet<StorySubPostCommentReaction> StorySubPostCommentReactions { get; set; }

    public virtual DbSet<StorySubPostReaction> StorySubPostReactions { get; set; }

    public virtual DbSet<StoryTagPost> StoryTagPosts { get; set; }

    public virtual DbSet<SystemResource> SystemResources { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    public virtual DbSet<SystemSettingHistory> SystemSettingHistories { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TagFavorite> TagFavorites { get; set; }

    //public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAuthenticator> UserAuthenticators { get; set; }

    public virtual DbSet<UserBlock> UserBlocks { get; set; }

    //public virtual DbSet<UserClaim> UserClaims { get; set; }

    public virtual DbSet<UserExclusiveSubPost> UserExclusiveSubPosts { get; set; }

    public virtual DbSet<UserFollow> UserFollows { get; set; }

    //public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserNameHistory> UserNameHistories { get; set; }

    public virtual DbSet<UserOtp> UserOtps { get; set; }

    public virtual DbSet<UserRecovery> UserRecoveries { get; set; }

    public virtual DbSet<UserReferral> UserReferrals { get; set; }

    public virtual DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    public virtual DbSet<UserRelation> UserRelations { get; set; }

    public virtual DbSet<UserSocial> UserSocials { get; set; }

    //public virtual DbSet<UserToken> UserTokens { get; set; }

    public virtual DbSet<ViewHistory> ViewHistories { get; set; }

    #endregion
}
