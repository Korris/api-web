using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for game."GamePostCommentReactions" (mirrors the Comic counterpart)
/// </summary>
public class GamePostCommentReactionConfiguration : BaseConfiguration<GamePostCommentReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<GamePostCommentReaction> builder)
    {
        builder.ToTable("GamePostCommentReactions", DbSchema.Game);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}
