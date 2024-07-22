using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicResourceConfiguration : BaseConfiguration<ComicResource>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicResource> builder)
    {
        builder.ToTable("ComicResources", DbSchema.Comic);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => x.HashId).IsUnique();
        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasOne(typeof(ComicSubPost)).WithMany().HasForeignKey("SubPostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
    }
}