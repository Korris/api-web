using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for game."GameResources" (mirrors ComicResourceConfiguration)
/// </summary>
public class GameResourceConfiguration : BaseConfiguration<GameResource>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<GameResource> builder)
    {
        builder.ToTable("GameResources", DbSchema.Game);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => x.HashId).IsUnique();
        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
