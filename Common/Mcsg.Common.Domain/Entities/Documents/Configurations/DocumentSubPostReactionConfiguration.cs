using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentSubPostReactionConfiguration : BaseConfiguration<DocumentSubPostReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentSubPostReaction> builder)
    {
        builder.ToTable("DocumentSubPostReactions", DbSchema.Document);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}