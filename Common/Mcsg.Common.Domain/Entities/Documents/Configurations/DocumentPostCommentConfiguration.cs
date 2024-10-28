using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentPostCommentConfiguration : BaseConfiguration<DocumentPostComment>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentPostComment> builder)
    {
        builder.ToTable("DocumentPostComments", DbSchema.Document);
        builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
        builder.HasOne(typeof(DocumentPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        builder.HasOne(typeof(DocumentResource)).WithMany().HasForeignKey("ResourceId");
    }
}