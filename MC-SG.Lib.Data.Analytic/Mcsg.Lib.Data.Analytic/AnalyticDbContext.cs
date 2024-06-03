using Mcsg.Lib.Data.Analytic.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Lib.Data.Analytic
{
    public class AnalyticDbContext : DbContext
    {
        public DbSet<UserViewPost> UserViewPosts { get; set; }
        public AnalyticDbContext(DbContextOptions<AnalyticDbContext> dbContext)
        : base(dbContext)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserViewPost>(entity =>
            {
                entity.ToTable("UserViewPosts");
                entity.Property(x => x.PostId).IsRequired();
                entity.HasIndex(x => new { x.PostId, x.SubPostId, x.AuthorId, x.UserId, x.UserHashString, x.CreatedDate }).IsUnique();
            });

        }
    }
}
