using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    using Mcsg.Lib.Data.Constants;

    public class ComicSubPostReactionConfiguration : BaseConfiguration<ComicSubPostReaction>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<ComicSubPostReaction> builder)
        {
            builder.ToTable("ComicSubPostReactions", DbSchema.Comic);
            builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(ComicSubPost)).WithMany().HasForeignKey("TargetId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}