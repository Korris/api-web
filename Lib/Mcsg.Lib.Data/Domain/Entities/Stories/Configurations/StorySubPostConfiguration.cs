using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    using Mcsg.Lib.Data.Constants;

    public class StorySubPostConfiguration : BaseConfiguration<StorySubPost>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<StorySubPost> builder)
        {
            builder.ToTable("StorySubPosts", DbSchema.Story);
            builder.HasIndex(x => new { x.PostId, x.HashId, x.AuthorId }).IsUnique();
            builder.HasOne(typeof(StoryPost)).WithMany().HasForeignKey("PostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
        }
    }
}