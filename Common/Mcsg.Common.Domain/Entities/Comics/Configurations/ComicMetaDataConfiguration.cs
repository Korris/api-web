using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicMetaDataConfiguration : BaseConfiguration<ComicMetaData>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicMetaData> builder)
    {
        builder.ToTable("ComicMetaDatas", DbSchema.Comic);
    }
}