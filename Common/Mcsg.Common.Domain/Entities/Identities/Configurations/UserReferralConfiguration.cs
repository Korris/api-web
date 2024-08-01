using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class UserReferralConfiguration : BaseConfiguration<UserReferral>
{

    public override void CreateEntityConfiguration(EntityTypeBuilder<UserReferral> builder)
    {
        builder.ToTable("UserReferrals", DbSchema.Identity);
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserReferrerId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserRefereeId");
    }
}