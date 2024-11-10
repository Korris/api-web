using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryPostLinkConfiguration : BaseConfiguration<StoryPostLink>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryPostLink> builder)
    {
        builder.ToTable("StoryPostLinks", DbSchema.Story);
    }
}
