using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class TagEntityConfiguration : BaseEntityConfiguration<Tag>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tags");
            builder.Property(x => x.Name).IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}