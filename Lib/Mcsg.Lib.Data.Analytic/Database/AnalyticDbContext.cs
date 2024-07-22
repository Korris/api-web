using Microsoft.EntityFrameworkCore;

namespace Mcsg.Lib.Data.Analytic;

using Entities;

/// <summary>
/// AnalyticDbContext
/// </summary>
public class AnalyticDbContext : DbContext
{
    #region -- Overrides --

    /// <summary>
    /// On model creating
    /// </summary>
    /// <param name="builder">Builder</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserViewPost>(entity =>
        {
            entity.ToTable("UserViewPosts");
            entity.Property(x => x.PostId).IsRequired();
            entity.HasIndex(x => new { x.PostId, x.SubPostId, x.AuthorId, x.UserId, x.UserHashString, x.CreatedOn }).IsUnique();
        });
    }

    #endregion

    #region -- Properties --

    public DbSet<UserViewPost> UserViewPosts { get; set; }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="options">Options</param>
    public AnalyticDbContext(DbContextOptions<AnalyticDbContext> options) : base(options) { }

    #endregion
}
