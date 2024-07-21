using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    using Mcsg.Lib.Data.Constants;

    public class StorySubPostCommentConfiguration : BaseConfiguration<StorySubPostComment>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<StorySubPostComment> builder)
        {
            builder.ToTable("StorySubPostComments", DbSchema.Story);
            builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(StorySubPost)).WithMany().HasForeignKey("PostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
            builder.HasOne(typeof(StoryResource)).WithMany().HasForeignKey("ResourceId");
        }
    }
}