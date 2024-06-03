using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class MetaDataEntityConfiguration : BaseEntityConfiguration<MetaData>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<MetaData> builder)
        {
            builder.ToTable("MetaDatas");
        }
    }
}