using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for tapshow."TapShowResources" (mirrors GameResourceConfiguration)
/// </summary>
public class TapShowResourceConfiguration : BaseConfiguration<TapShowResource>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<TapShowResource> builder)
    {
        builder.ToTable("TapShowResources", DbSchema.TapShow);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => x.HashId).IsUnique();
        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
