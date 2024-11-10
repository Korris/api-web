using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialPostConfiguration : BaseConfiguration<SocialPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialPost> builder)
    {
        builder.ToTable("SocialPosts", DbSchema.Social);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => new { x.HashId, x.UserId, x.Type, x.Id }).IsUnique();
    }
}