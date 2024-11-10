using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentPostFavoriteConfiguration : BaseConfiguration<DocumentPostFavorite>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentPostFavorite> builder)
    {
        builder.ToTable("DocumentPostFavorites", DbSchema.Document);
    }
}
