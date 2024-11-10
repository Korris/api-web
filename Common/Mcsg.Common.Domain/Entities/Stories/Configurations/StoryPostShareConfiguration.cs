using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryPostShareConfiguration : BaseConfiguration<StoryPostShare>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryPostShare> builder)
    {
        builder.ToTable("StoryPostShares", DbSchema.Story);
    }
}
