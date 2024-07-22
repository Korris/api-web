using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations
{
    public class UserExclusiveSubPostConfiguration : BaseConfiguration<UserExclusiveSubPost>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<UserExclusiveSubPost> builder)
        {
            builder.ToTable("UserExclusiveSubPosts");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
            builder.HasOne(typeof(SubPost)).WithMany().HasForeignKey("SubPostId");
        }
    }
}