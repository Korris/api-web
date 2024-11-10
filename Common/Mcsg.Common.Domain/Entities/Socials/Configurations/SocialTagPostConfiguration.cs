using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialTagPostConfiguration : BaseConfiguration<SocialTagPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialTagPost> builder)
    {
        builder.ToTable("SocialTagPosts", DbSchema.Social);
        builder.HasIndex(x => new { x.TagId, x.PostId });
    }
}