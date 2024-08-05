using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialPostHideConfiguration : BaseConfiguration<SocialPostHide>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialPostHide> builder)
    {
        builder.ToTable("SocialPostHides", DbSchema.Social);
        builder.HasOne(typeof(SocialPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
