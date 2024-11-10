using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentReportConfiguration : BaseConfiguration<DocumentReport>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentReport> builder)
    {
        builder.ToTable("DocumentReports", DbSchema.Document);
    }
}
