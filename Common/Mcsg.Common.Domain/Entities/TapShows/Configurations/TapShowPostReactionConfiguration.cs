using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for tapshow."TapShowPostReactions" (mirrors the Game counterpart)
/// </summary>
public class TapShowPostReactionConfiguration : BaseConfiguration<TapShowPostReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<TapShowPostReaction> builder)
    {
        builder.ToTable("TapShowPostReactions", DbSchema.TapShow);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}
