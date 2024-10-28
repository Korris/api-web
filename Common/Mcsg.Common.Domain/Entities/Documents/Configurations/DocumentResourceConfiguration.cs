using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentResourceConfiguration : BaseConfiguration<DocumentResource>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentResource> builder)
    {
        builder.ToTable("DocumentResources", DbSchema.Document);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => x.HashId).IsUnique();
        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasOne(typeof(DocumentSubPost)).WithMany().HasForeignKey("SubPostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
    }
}