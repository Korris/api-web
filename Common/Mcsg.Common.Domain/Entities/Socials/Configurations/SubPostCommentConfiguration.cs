using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

public class SubPostCommentConfiguration : BaseConfiguration<SubPostComment>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SubPostComment> builder)
    {
        builder.ToTable("SubPostComments");
        builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
        builder.HasOne(typeof(SubPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        builder.HasOne(typeof(Resource)).WithMany().HasForeignKey("ResourceId");
    }
}