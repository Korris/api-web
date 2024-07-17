using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class UserRefreshTokenConfiguration : BaseConfiguration<UserRefreshToken>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.ToTable("UserRefreshTokens");
            builder.Property(x => x.RefreshToken).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.RefreshTokenExpiryTime);
        }
    }
}