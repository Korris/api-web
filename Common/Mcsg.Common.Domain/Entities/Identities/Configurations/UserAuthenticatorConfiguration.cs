using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class UserAuthenticatorConfiguration : BaseConfiguration<UserAuthenticator>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<UserAuthenticator> builder)
    {
        builder.ToTable("UserAuthenticators", DbSchema.Identity);
        builder.Property(x => x.UserId).IsRequired();
    }
}