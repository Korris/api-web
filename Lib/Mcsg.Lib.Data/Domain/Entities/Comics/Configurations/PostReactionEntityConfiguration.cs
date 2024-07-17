using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Comics.Configurations
{
    using Mcsg.Lib.Data.Constants;
    using Mcsg.Lib.Data.Domain.Entities.Configurations;

    public class PostReactionEntityConfiguration : BaseEntityConfiguration<PostReaction>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<PostReaction> builder)
        {
            builder.ToTable("PostReactions", DbSchema.Comic);
            builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(Post)).WithMany().HasForeignKey("TargetId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}