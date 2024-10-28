using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentPostConfiguration : BaseConfiguration<DocumentPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentPost> builder)
    {
        builder.ToTable("DocumentPosts", DbSchema.Document);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => new { x.HashId, x.UserId, x.Type, x.Id }).IsUnique();
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}