using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class JobEntityConfiguration : BaseEntityConfiguration<Job>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("Jobs");
        }
    }
}