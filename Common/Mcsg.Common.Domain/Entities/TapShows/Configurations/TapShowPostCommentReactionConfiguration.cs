using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for tapshow."TapShowPostCommentReactions" (mirrors the Game counterpart)
/// </summary>
public class TapShowPostCommentReactionConfiguration : BaseConfiguration<TapShowPostCommentReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<TapShowPostCommentReaction> builder)
    {
        builder.ToTable("TapShowPostCommentReactions", DbSchema.TapShow);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}
