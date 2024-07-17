using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class BackgroundMediaConfiguration : BaseConfiguration<BackgroundMedia>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<BackgroundMedia> builder)
        {
            builder.ToTable("BackgroundMedias");
        }
    }
}
