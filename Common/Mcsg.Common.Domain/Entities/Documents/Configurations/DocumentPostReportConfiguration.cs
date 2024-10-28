using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentPostReportConfiguration : BaseConfiguration<DocumentPostReport>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentPostReport> builder)
    {
        builder.ToTable("DocumentPostReports", DbSchema.Document);
        builder.HasOne(typeof(DocumentPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
