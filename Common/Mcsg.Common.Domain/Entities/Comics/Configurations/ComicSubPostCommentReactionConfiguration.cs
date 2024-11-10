using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicSubPostCommentReactionConfiguration : BaseConfiguration<ComicSubPostCommentReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicSubPostCommentReaction> builder)
    {
        builder.ToTable("ComicSubPostCommentReactions", DbSchema.Comic);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}