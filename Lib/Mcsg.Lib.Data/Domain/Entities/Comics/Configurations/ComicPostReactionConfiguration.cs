using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Comics.Configurations
{
    using Mcsg.Lib.Data.Constants;
    using Mcsg.Lib.Data.Domain.Entities.Configurations;

    public class ComicPostReactionConfiguration : BaseConfiguration<ComicPostReaction>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<ComicPostReaction> builder)
        {
            builder.ToTable("ComicPostReactions", DbSchema.Comic);
            builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(ComicPost)).WithMany().HasForeignKey("TargetId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        }
    }
}