using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Documents.Configurations;

using Core.Constants;

public class DocumentReportDetailConfiguration : BaseConfiguration<DocumentReportDetail>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentReportDetail> builder)
    {
        builder.ToTable("DocumentReportDetails", DbSchema.Document);
        builder.HasOne(typeof(DocumentReport)).WithMany().HasForeignKey("ReportId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
