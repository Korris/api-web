using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryMetaDataConfiguration : BaseConfiguration<StoryMetaData>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryMetaData> builder)
    {
        builder.ToTable("StoryMetaDatas", DbSchema.Story);
    }
}