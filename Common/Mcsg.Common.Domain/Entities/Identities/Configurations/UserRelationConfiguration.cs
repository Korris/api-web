using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class UserRelationConfiguration : BaseConfiguration<UserRelation>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<UserRelation> builder)
    {
        builder.ToTable("UserRelations", DbSchema.Identity);
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId1");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId2");
    }
}