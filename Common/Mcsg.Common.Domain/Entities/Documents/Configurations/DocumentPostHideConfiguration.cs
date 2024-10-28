using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentPostHideConfiguration : BaseConfiguration<DocumentPostHide>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentPostHide> builder)
    {
        builder.ToTable("DocumentPostHides", DbSchema.Document);
        builder.HasOne(typeof(DocumentPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
