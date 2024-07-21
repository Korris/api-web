using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    using Mcsg.Lib.Data.Constants;

    public class StoryResourceConfiguration : BaseConfiguration<StoryResource>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<StoryResource> builder)
        {
            builder.ToTable("StoryResources", DbSchema.Story);
            builder.Property(x => x.HashId).IsRequired();
            builder.HasIndex(x => x.HashId).IsUnique();
            builder.Property(x => x.Name).IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasOne(typeof(StorySubPost)).WithMany().HasForeignKey("SubPostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}