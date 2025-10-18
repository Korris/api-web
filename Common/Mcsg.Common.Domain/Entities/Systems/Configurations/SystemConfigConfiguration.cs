using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SystemConfigConfiguration : BaseConfiguration<SystemConfig>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SystemConfig> builder)
    {
        builder.ToTable("SystemConfigs", DbSchema.System);
        builder.Property(x => x.Key).IsRequired();
        builder.HasIndex(x => x.Key).IsUnique();
    }
}