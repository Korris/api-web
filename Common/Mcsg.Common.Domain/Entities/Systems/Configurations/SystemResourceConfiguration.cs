using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SystemResourceConfiguration : BaseConfiguration<SystemResource>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SystemResource> builder)
    {
        builder.ToTable("SystemResources", DbSchema.System);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => x.HashId).IsUnique();
        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}