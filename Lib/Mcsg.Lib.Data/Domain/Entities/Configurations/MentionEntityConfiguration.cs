using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class MentionEntityConfiguration : BaseEntityConfiguration<Mention>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<Mention> builder)
        {
            builder.ToTable("Mentions");
        }
    }
}
