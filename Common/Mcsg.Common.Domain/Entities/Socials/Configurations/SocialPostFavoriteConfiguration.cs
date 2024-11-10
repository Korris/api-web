using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialPostFavoriteConfiguration : BaseConfiguration<SocialPostFavorite>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialPostFavorite> builder)
    {
        builder.ToTable("SocialPostFavorites", DbSchema.Social);
    }
}
