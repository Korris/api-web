using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    using Mcsg.Lib.Data.Constants;

    public class StoryPostReactionConfiguration : BaseConfiguration<StoryPostReaction>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<StoryPostReaction> builder)
        {
            builder.ToTable("StoryPostReactions", DbSchema.Story);
            builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(StoryPost)).WithMany().HasForeignKey("TargetId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}