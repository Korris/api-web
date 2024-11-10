using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicPostFavoriteConfiguration : BaseConfiguration<ComicPostFavorite>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicPostFavorite> builder)
    {
        builder.ToTable("ComicPostFavorites", DbSchema.Comic);
    }
}
