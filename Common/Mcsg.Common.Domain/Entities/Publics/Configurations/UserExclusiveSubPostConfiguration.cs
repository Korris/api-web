using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

public class UserExclusiveSubPostConfiguration : BaseConfiguration<UserExclusiveSubPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<UserExclusiveSubPost> builder)
    {
        builder.ToTable("UserExclusiveSubPosts");
    }
}