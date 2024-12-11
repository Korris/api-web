using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class UserRecoveryConfiguration : BaseConfiguration<UserRecovery>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<UserRecovery> builder)
    {
        builder.ToTable("UserRecoveries", DbSchema.Identity);
        builder.Property(x => x.UserId).IsRequired();
    }
}