using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Lib.Data;

using Domain.Entities;
using Domain.Entities.Configurations;

/// <summary>
/// McsgDbContext
/// </summary>
public class McsgDbContext : IdentityDbContext<User, Role, Guid>
{
    #region -- Overrides --

    /// <summary>
    /// On model creating
    /// </summary>
    /// <param name="builder">Builder</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(SessionEntityConfiguration).Assembly);

        builder.Entity<User>(entity =>
        {
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.ToTable("Users");
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
            entity.ToTable("UserClaims");
        });

        builder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        builder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("UserTokens"));

        builder.Entity<Role>(entity =>
        {
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.ToTable("Roles");
        });

        builder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        builder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("UserRoles"));

        //Seed data
        //DataSeeder.Seed(builder);
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public McsgDbContext() { }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="options">Options</param>
    public McsgDbContext(DbContextOptions<McsgDbContext> options) : base(options) { }

    #endregion

    #region -- Properties --

    public DbSet<Session> Sessions { get; set; }
    public DbSet<UserOtp> UserOtps { get; set; }
    public DbSet<UserSocial> UserSocials { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<SubPost> SubPosts { get; set; }
    public DbSet<PostComment> PostComments { get; set; }
    public DbSet<PostComment> SubPostComments { get; set; }
    public DbSet<PostReaction> PostReactions { get; set; }
    public DbSet<SubPostReaction> SubPostReactions { get; set; }
    public DbSet<PostCommentReaction> PostCommentReactions { get; set; }
    public DbSet<SubPostCommentReaction> SubPostCommentReactions { get; set; }
    public DbSet<MetaData> MetaDatas { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<SystemSettingHistory> SystemSettingHistories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagPost> TagPosts { get; set; }
    public DbSet<UserFollow> UserFollows { get; set; }
    public DbSet<TagFavorite> TagFavorites { get; set; }
    public DbSet<PostFavorite> PostFavorites { get; set; }
    public DbSet<SmartLookup> SmartLookups { get; set; }
    public DbSet<SmartCountAction> SmartCountActions { get; set; }
    public DbSet<SmartLookupUser> SmartLookupUsers { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationObject> NotificationObjects { get; set; }
    public DbSet<BackgroundMedia> BackgroundMedias { get; set; }
    public DbSet<BackgroundMediaPost> BackgroundMediaPosts { get; set; }
    public DbSet<Mention> Mentions { get; set; }
    public DbSet<CrawComic> CrawComics { get; set; }
    public DbSet<CrawComicChapter> CrawComicChapters { get; set; }
    public DbSet<PostReport> PostReports { get; set; }
    public DbSet<PostLink> PostLinks { get; set; }

    #endregion
}
