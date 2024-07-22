using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations
{
    public class PostReactionConfiguration : BaseConfiguration<PostReaction>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<PostReaction> builder)
        {
            builder.ToTable("PostReactions");
            builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(Post)).WithMany().HasForeignKey("TargetId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}