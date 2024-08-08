using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SystemSettingHistoryConfiguration : BaseConfiguration<SystemSettingHistory>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SystemSettingHistory> builder)
    {
        builder.ToTable("SystemSettingHistories", DbSchema.System);
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
        builder.HasOne(typeof(SystemSetting)).WithMany().HasForeignKey("SystemSettingId");
    }
}