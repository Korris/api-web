using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations
{
    public class TagPostConfiguration : BaseConfiguration<TagPost>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<TagPost> builder)
        {
            builder.ToTable("TagPosts");
            builder.HasIndex(x => new { x.TagId, x.PostId });
            builder.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagId");
            builder.HasOne(typeof(Post)).WithMany().HasForeignKey("PostId");
        }
    }
}