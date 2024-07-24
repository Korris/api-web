using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class UserSocialConfiguration : BaseConfiguration<UserSocial>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<UserSocial> builder)
    {
        builder.ToTable("UserSocials", DbSchema.Identity);
    }
}