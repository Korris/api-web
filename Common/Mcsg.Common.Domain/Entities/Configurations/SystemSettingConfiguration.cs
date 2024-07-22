using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

public class SystemSettingConfiguration : BaseConfiguration<SystemSetting>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings");
        builder.Property(x => x.Key).IsRequired();
        builder.HasIndex(x => x.Key).IsUnique();
    }
}