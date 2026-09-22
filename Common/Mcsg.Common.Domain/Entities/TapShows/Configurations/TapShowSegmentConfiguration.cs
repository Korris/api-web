using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for tapshow."TapShowSegments"
/// </summary>
public class TapShowSegmentConfiguration : BaseConfiguration<TapShowSegment>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<TapShowSegment> builder)
    {
        builder.ToTable("TapShowSegments", DbSchema.TapShow);
        builder.HasIndex(x => new { x.ChapterId, x.Order });
    }
}
