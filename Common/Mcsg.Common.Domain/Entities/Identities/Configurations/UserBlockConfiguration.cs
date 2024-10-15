using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class UserBlockConfiguration : BaseConfiguration<UserBlock>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<UserBlock> builder)
    {
        builder.ToTable("UserBlocks", DbSchema.Identity);
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId1");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId2");
    }
}