using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryPostHideConfiguration : BaseConfiguration<StoryPostHide>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryPostHide> builder)
    {
        builder.ToTable("StoryPostHides", DbSchema.Story);
    }
}
