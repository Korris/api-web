using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    using Mcsg.Lib.Data.Constants;

    public class StorySubPostReactionConfiguration : BaseConfiguration<StorySubPostReaction>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<StorySubPostReaction> builder)
        {
            builder.ToTable("StorySubPostReactions", DbSchema.Story);
            builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(StorySubPost)).WithMany().HasForeignKey("TargetId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}