using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class UserSocialEntityConfiguration : BaseEntityConfiguration<UserSocial>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<UserSocial> builder)
        {
            builder.ToTable("UserSocials");
        }
    }
}