using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    using Mcsg.Lib.Data.Constants;

    public class StoryTagPostConfiguration : BaseConfiguration<StoryTagPost>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<StoryTagPost> builder)
        {
            builder.ToTable("StoryTagPosts", DbSchema.Story);
            builder.HasIndex(x => new { x.TagId, x.PostId });
            builder.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagId");
            builder.HasOne(typeof(StoryPost)).WithMany().HasForeignKey("PostId");
        }
    }
}