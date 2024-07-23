using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialResourceConfiguration : BaseConfiguration<SocialResource>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialResource> builder)
    {
        builder.ToTable("SocialResources", DbSchema.Social);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => x.HashId).IsUnique();
        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasOne(typeof(SocialSubPost)).WithMany().HasForeignKey("SubPostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
    }
}