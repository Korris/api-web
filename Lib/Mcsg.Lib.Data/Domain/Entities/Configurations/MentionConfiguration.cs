using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class MentionConfiguration : BaseConfiguration<Mention>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<Mention> builder)
        {
            builder.ToTable("Mentions");
        }
    }
}
