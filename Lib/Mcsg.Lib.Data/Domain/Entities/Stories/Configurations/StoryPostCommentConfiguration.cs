using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Stories.Configurations
{
    using Mcsg.Lib.Data.Constants;
    using Mcsg.Lib.Data.Domain.Entities.Configurations;

    public class StoryPostCommentConfiguration : BaseConfiguration<StoryPostComment>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<StoryPostComment> builder)
        {
            builder.ToTable("StoryPostComments", DbSchema.Story);
            builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(StoryPost)).WithMany().HasForeignKey("PostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
            builder.HasOne(typeof(StoryResource)).WithMany().HasForeignKey("ResourceId");
        }
    }
}