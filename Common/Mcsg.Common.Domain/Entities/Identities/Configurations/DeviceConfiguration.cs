using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DeviceConfiguration : BaseConfiguration<Device>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices", DbSchema.Identity);
    }
}
