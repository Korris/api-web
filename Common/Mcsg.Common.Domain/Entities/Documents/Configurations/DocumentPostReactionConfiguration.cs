using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentPostReactionConfiguration : BaseConfiguration<DocumentPostReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentPostReaction> builder)
    {
        builder.ToTable("DocumentPostReactions", DbSchema.Document);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}