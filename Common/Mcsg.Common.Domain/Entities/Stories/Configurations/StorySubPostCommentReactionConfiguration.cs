using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    using Mcsg.Lib.Data.Constants;

    public class StorySubPostCommentReactionConfiguration : BaseConfiguration<StorySubPostCommentReaction>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<StorySubPostCommentReaction> builder)
        {
            builder.ToTable("StorySubPostCommentReactions", DbSchema.Story);
            builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(StorySubPostComment)).WithMany().HasForeignKey("TargetId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}