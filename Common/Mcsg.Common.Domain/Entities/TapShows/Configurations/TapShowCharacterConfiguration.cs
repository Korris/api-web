using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for tapshow."TapShowCharacters"
/// </summary>
public class TapShowCharacterConfiguration : BaseConfiguration<TapShowCharacter>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<TapShowCharacter> builder)
    {
        builder.ToTable("TapShowCharacters", DbSchema.TapShow);
        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => new { x.PostId, x.Order });
    }
}
