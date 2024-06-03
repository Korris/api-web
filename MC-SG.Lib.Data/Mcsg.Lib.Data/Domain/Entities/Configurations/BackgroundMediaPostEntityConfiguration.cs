using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class BackgroundMediaPostEntityConfiguration : BaseEntityConfiguration<BackgroundMediaPost>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<BackgroundMediaPost> builder)
        {
            builder.ToTable("BackgroundMediaPosts");
            builder.HasOne(typeof(BackgroundMedia)).WithMany().HasForeignKey("BackgroundMediaId");
            builder.HasOne(typeof(Post)).WithMany().HasForeignKey("PostId");
        }
    }
}
