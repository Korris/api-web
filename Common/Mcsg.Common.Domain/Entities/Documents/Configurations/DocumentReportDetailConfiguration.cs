using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentReportDetailConfiguration : BaseConfiguration<DocumentReportDetail>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentReportDetail> builder)
    {
        builder.ToTable("DocumentReportDetails", DbSchema.Document);
    }
}
