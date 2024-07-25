using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryFollowedPostConfiguration : BaseConfiguration<StoryFollowedPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryFollowedPost> builder)
    {
        builder.ToTable("StoryFollowedPosts", DbSchema.Story);
        builder.HasIndex(x => new { x.PostId, x.CreatedBy, x.Id }).IsUnique();
        builder.HasOne(typeof(StoryPost)).WithMany().HasForeignKey("PostId");

    }
}