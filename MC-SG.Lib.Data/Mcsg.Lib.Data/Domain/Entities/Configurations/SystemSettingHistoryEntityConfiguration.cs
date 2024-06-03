using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class SystemSettingHistoryEntityConfiguration : BaseEntityConfiguration<SystemSettingHistory>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<SystemSettingHistory> builder)
        {
            builder.ToTable("SystemSettingHistories");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
            builder.HasOne(typeof(SystemSetting)).WithMany().HasForeignKey("SystemSettingId");
        }
    }
}