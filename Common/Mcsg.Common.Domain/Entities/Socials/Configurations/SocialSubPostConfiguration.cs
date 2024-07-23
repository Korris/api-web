using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialSubPostConfiguration : BaseConfiguration<SocialSubPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialSubPost> builder)
    {
        builder.ToTable("SocialSubPosts", DbSchema.Social);
        builder.HasIndex(x => new { x.PostId, x.HashId, x.AuthorId }).IsUnique();
        builder.HasOne(typeof(SocialPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}