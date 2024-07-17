using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Comics.Configurations
{
    using Mcsg.Lib.Data.Constants;
    using Mcsg.Lib.Data.Domain.Entities.Configurations;

    public class SubPostCommentReactionEntityConfiguration : BaseEntityConfiguration<SubPostCommentReaction>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<SubPostCommentReaction> builder)
        {
            builder.ToTable("SubPostCommentReactions", DbSchema.Comic);
            builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(SubPostComment)).WithMany().HasForeignKey("TargetId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}