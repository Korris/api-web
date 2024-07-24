using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class UserRefreshTokenConfiguration : BaseConfiguration<UserRefreshToken>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.ToTable("UserRefreshTokens", DbSchema.Identity);
        builder.Property(x => x.RefreshToken).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.RefreshTokenExpiryTime);
    }
}