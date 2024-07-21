using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    using Mcsg.Lib.Data.Constants;

    public class StoryPostCommentReactionConfiguration : BaseConfiguration<StoryPostCommentReaction>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<StoryPostCommentReaction> builder)
        {
            builder.ToTable("StoryPostCommentReactions", DbSchema.Story);
            builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(StoryPostComment)).WithMany().HasForeignKey("TargetId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}