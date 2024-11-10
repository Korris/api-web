using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryTagPostConfiguration : BaseConfiguration<StoryTagPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryTagPost> builder)
    {
        builder.ToTable("StoryTagPosts", DbSchema.Story);
        builder.HasIndex(x => new { x.TagId, x.PostId });
    }
}