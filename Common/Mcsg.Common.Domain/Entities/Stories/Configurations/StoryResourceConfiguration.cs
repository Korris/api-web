using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryResourceConfiguration : BaseConfiguration<StoryResource>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryResource> builder)
    {
        builder.ToTable("StoryResources", DbSchema.Story);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => x.HashId).IsUnique();
        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}