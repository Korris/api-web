using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class UserFollowConfiguration : BaseConfiguration<UserFollow>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<UserFollow> builder)
    {
        builder.ToTable("UserFollows", DbSchema.Identity);
    }
}