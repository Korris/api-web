using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicPostLinkConfiguration : BaseConfiguration<ComicPostLink>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicPostLink> builder)
    {
        builder.ToTable("ComicPostLinks", DbSchema.Comic);
    }
}
