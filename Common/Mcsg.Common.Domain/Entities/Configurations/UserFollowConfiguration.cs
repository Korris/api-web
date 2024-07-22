using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations
{
    public class UserFollowConfiguration : BaseConfiguration<UserFollow>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<UserFollow> builder)
        {
            builder.ToTable("UserFollows");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserFollowerId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserFollowingId");
        }
    }
}