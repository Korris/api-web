using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class BackgroundMediaEntityConfiguration : BaseEntityConfiguration<BackgroundMedia>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<BackgroundMedia> builder)
        {
            builder.ToTable("BackgroundMedias");
        }
    }
}
