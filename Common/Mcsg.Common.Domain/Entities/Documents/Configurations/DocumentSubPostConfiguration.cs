using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentSubPostConfiguration : BaseConfiguration<DocumentSubPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentSubPost> builder)
    {
        builder.ToTable("DocumentSubPosts", DbSchema.Document);
        builder.HasIndex(x => new { x.PostId, x.HashId, x.AuthorId }).IsUnique();
    }
}