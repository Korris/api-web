using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for tapshow."TapShowSegmentChoices".
/// Two FKs to the same table (from Segment → TargetSegment); both cascade so deleting a segment removes its branches.
/// </summary>
public class TapShowSegmentChoiceConfiguration : BaseConfiguration<TapShowSegmentChoice>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<TapShowSegmentChoice> builder)
    {
        builder.ToTable("TapShowSegmentChoices", DbSchema.TapShow);
        builder.HasIndex(x => new { x.SegmentId, x.Order });
        builder.HasIndex(x => x.TargetSegmentId);

        builder.HasOne(x => x.Segment)
            .WithMany(x => x.Choices)
            .HasForeignKey(x => x.SegmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TargetSegment)
            .WithMany(x => x.IncomingChoices)
            .HasForeignKey(x => x.TargetSegmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
