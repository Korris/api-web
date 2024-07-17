using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations;

using Constants;

public class UserNameHistoryEntityConfiguration : BaseEntityConfiguration<UserNameHistory>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<UserNameHistory> builder)
    {
        builder.ToTable("UserNameHistories", DbSchema.Identity);
    }
}
