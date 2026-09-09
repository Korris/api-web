using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for game."GamePostReactions" (mirrors the Comic counterpart)
/// </summary>
public class GamePostReactionConfiguration : BaseConfiguration<GamePostReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<GamePostReaction> builder)
    {
        builder.ToTable("GamePostReactions", DbSchema.Game);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}
