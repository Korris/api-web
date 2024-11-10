using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

public class TagFavoriteConfiguration : BaseConfiguration<TagFavorite>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<TagFavorite> builder)
    {
        builder.ToTable("TagFavorites");
    }
}
