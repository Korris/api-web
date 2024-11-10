using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicPostShareConfiguration : BaseConfiguration<ComicPostShare>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicPostShare> builder)
    {
        builder.ToTable("ComicPostShares", DbSchema.Comic);
    }
}
