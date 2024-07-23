using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

public class BackgroundMediaPostConfiguration : BaseConfiguration<BackgroundMediaPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<BackgroundMediaPost> builder)
    {
        builder.ToTable("BackgroundMediaPosts");
        builder.HasOne(typeof(BackgroundMedia)).WithMany().HasForeignKey("BackgroundMediaId");
        builder.HasOne(typeof(SocialPost)).WithMany().HasForeignKey("PostId");
    }
}
