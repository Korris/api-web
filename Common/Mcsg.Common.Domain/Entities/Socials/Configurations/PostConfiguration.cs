using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

public class PostConfiguration : BaseConfiguration<Post>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => new { x.HashId, x.UserId, x.Type, x.Id }).IsUnique();
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}