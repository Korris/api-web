using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentSubPostCommentReactionConfiguration : BaseConfiguration<DocumentSubPostCommentReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentSubPostCommentReaction> builder)
    {
        builder.ToTable("DocumentSubPostCommentReactions", DbSchema.Document);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
        builder.HasOne(typeof(DocumentSubPostComment)).WithMany().HasForeignKey("TargetId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
    }
}