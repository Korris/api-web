using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Common.Domain;

using Core.Constants;
using Domain.Entities;
using Domain.Entities.Configurations;
using SeedWork.Extensions;

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

        builder.ApplyConfigurationsFromAssembly(typeof(SessionConfiguration).Assembly);

        builder.Entity<User>(entity =>
        {
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.ToTable("Users", DbSchema.Identity);
            entity.Property(e => e.FirstName)
            .IsRequired(false)
            .HasMaxLength(256);

            entity.Property(e => e.LastName)
            .IsRequired(false)
            .HasMaxLength(256);

            entity.Property(e => e.Avatar)
            .IsRequired(false)
            .HasMaxLength(500);

            entity.Property(e => e.ProfileName)
            .IsRequired(false)
            .HasMaxLength(256);

            entity.HasIndex(x => x.ReferralCode);
        });

        builder.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("UserClaims", DbSchema.Identity);
        });

        builder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("UserLogins", DbSchema.Identity);
        });

        builder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("UserTokens", DbSchema.Identity));

        builder.Entity<Role>(entity =>
        {
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.ToTable("Roles", DbSchema.Identity);
        });

        builder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("RoleClaims", DbSchema.Identity);
        });

        builder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("UserRoles", DbSchema.Identity));

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

    /// <summary>
    /// Make serial number
    /// </summary>
    /// <param name="q">Queryable</param>
    /// <param name="sOrderBy">Selector for OrderBy statement</param>
    /// <param name="sSelect">Selector for Select statement</param>
    /// <param name="prefix">Prefix</param>
    /// <returns>Return the result</returns>
    public string MakeNo<T>(IQueryable<T> q, Func<T, Guid> sOrderBy, Func<T, string> sSelect, string prefix)
    {
        // Prefix
        if (string.IsNullOrWhiteSpace(prefix))
        {
            prefix = "UN";
        }
        else
        {
            prefix = prefix.Trim();
        }
        var ym = DateTime.Now.ToString("yyyyMM");
        prefix += ym + "-{0:0000#}";

        // First
        var m = q.OrderBy(sOrderBy).Select(sSelect).LastOrDefault();
        if (m == null)
        {
            return string.Format(prefix, 1);
        }

        // Next
        var arr = m.Split('-').LastOrDefault();
        var num = arr == null ? "0" : arr.ToNumber();
        var seq = Convert.ToUInt32(num) + 1;
        return string.Format(prefix, seq);
    }

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

    public virtual DbSet<ComicPostReport> ComicPostReports { get; set; }

    public virtual DbSet<ComicResource> ComicResources { get; set; }

    public virtual DbSet<ComicSubPost> ComicSubPosts { get; set; }

    public virtual DbSet<ComicSubPostComment> ComicSubPostComments { get; set; }

    public virtual DbSet<ComicSubPostCommentReaction> ComicSubPostCommentReactions { get; set; }

    public virtual DbSet<ComicSubPostReaction> ComicSubPostReactions { get; set; }

    public virtual DbSet<ComicTagPost> ComicTagPosts { get; set; }

    public virtual DbSet<CrawComic> CrawComics { get; set; }

    public virtual DbSet<CrawComicChapter> CrawComicChapters { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Mention> Mentions { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationObject> NotificationObjects { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    //public virtual DbSet<Role> Roles { get; set; }

    //public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

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

    public virtual DbSet<SocialPostReport> SocialPostReports { get; set; }

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

    public virtual DbSet<StoryPostReport> StoryPostReports { get; set; }

    public virtual DbSet<StoryResource> StoryResources { get; set; }

    public virtual DbSet<StorySubPost> StorySubPosts { get; set; }

    public virtual DbSet<StorySubPostComment> StorySubPostComments { get; set; }

    public virtual DbSet<StorySubPostCommentReaction> StorySubPostCommentReactions { get; set; }

    public virtual DbSet<StorySubPostReaction> StorySubPostReactions { get; set; }

    public virtual DbSet<StoryTagPost> StoryTagPosts { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    public virtual DbSet<SystemSettingHistory> SystemSettingHistories { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TagFavorite> TagFavorites { get; set; }

    //public virtual DbSet<User> Users { get; set; }

    //public virtual DbSet<UserClaim> UserClaims { get; set; }

    public virtual DbSet<UserExclusiveSubPost> UserExclusiveSubPosts { get; set; }

    public virtual DbSet<UserFollow> UserFollows { get; set; }

    //public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserNameHistory> UserNameHistories { get; set; }

    public virtual DbSet<UserOtp> UserOtps { get; set; }

    public virtual DbSet<UserReferral> UserReferrals { get; set; }

    public virtual DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    public virtual DbSet<UserRelation> UserRelations { get; set; }

    public virtual DbSet<UserSocial> UserSocials { get; set; }

    //public virtual DbSet<UserToken> UserTokens { get; set; }

    public virtual DbSet<ViewHistory> ViewHistories { get; set; }

    #endregion
}
