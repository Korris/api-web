using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentTagPostConfiguration : BaseConfiguration<DocumentTagPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentTagPost> builder)
    {
        builder.ToTable("DocumentTagPosts", DbSchema.Document);
        builder.HasIndex(x => new { x.TagId, x.PostId });
    }
}