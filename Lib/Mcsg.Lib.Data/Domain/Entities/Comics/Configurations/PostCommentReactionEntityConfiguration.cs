using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Comics.Configurations
{
    using Mcsg.Lib.Data.Constants;
    using Mcsg.Lib.Data.Domain.Entities.Configurations;

    public class PostCommentReactionEntityConfiguration : BaseEntityConfiguration<PostCommentReaction>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<PostCommentReaction> builder)
        {
            builder.ToTable("PostCommentReactions", DbSchema.Comic);
            builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(PostComment)).WithMany().HasForeignKey("TargetId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}