using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentSubPostCommentConfiguration : BaseConfiguration<DocumentSubPostComment>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentSubPostComment> builder)
    {
        builder.ToTable("DocumentSubPostComments", DbSchema.Document);
        builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
    }
}