using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class UserOtpConfiguration : BaseConfiguration<UserOtp>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<UserOtp> builder)
    {
        builder.ToTable("UserOtps", DbSchema.Identity);
    }
}