using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations
{
    using Core.Constants;

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