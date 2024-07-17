using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Comics.Configurations
{
    using Mcsg.Lib.Data.Constants;
    using Mcsg.Lib.Data.Domain.Entities.Configurations;

    public class SubPostCommentEntityConfiguration : BaseEntityConfiguration<SubPostComment>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<SubPostComment> builder)
        {
            builder.ToTable("SubPostComments", DbSchema.Comic);
            builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(SubPost)).WithMany().HasForeignKey("PostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
            builder.HasOne(typeof(Resource)).WithMany().HasForeignKey("ResourceId");
        }
    }
}