using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentPostCommentReactionConfiguration : BaseConfiguration<DocumentPostCommentReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentPostCommentReaction> builder)
    {
        builder.ToTable("DocumentPostCommentReactions", DbSchema.Document);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}