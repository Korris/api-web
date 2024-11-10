using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialPostLinkConfiguration : BaseConfiguration<SocialPostLink>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialPostLink> builder)
    {
        builder.ToTable("SocialPostLinks", DbSchema.Social);
    }
}
