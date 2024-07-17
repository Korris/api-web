using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Comics.Configurations
{
    using Mcsg.Lib.Data.Constants;
    using Mcsg.Lib.Data.Domain.Entities.Configurations;

    public class ResourceEntityConfiguration : BaseEntityConfiguration<Resource>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<Resource> builder)
        {
            builder.ToTable("Resources", DbSchema.Comic);
            builder.Property(x => x.HashId).IsRequired();
            builder.HasIndex(x => x.HashId).IsUnique();
            builder.Property(x => x.Name).IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasOne(typeof(SubPost)).WithMany().HasForeignKey("SubPostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}