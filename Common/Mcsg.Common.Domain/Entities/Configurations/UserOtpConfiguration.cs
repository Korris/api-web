using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class UserOtpConfiguration : BaseConfiguration<UserOtp>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<UserOtp> builder)
        {
            builder.ToTable("UserOtps");
        }
    }
}