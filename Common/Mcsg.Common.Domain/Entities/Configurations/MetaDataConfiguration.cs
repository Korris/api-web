using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

public class MetaDataConfiguration : BaseConfiguration<MetaData>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<MetaData> builder)
    {
        builder.ToTable("MetaDatas");
    }
}