using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentPostLinkConfiguration : BaseConfiguration<DocumentPostLink>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentPostLink> builder)
    {
        builder.ToTable("DocumentPostLinks", DbSchema.Document);
        builder.HasOne(typeof(DocumentPost)).WithMany().HasForeignKey("PostId");
    }
}
