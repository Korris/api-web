using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryPostFavoriteConfiguration : BaseConfiguration<StoryPostFavorite>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryPostFavorite> builder)
    {
        builder.ToTable("StoryPostFavorites", DbSchema.Story);
    }
}
