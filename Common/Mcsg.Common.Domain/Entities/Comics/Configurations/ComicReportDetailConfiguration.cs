using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicReportDetailConfiguration : BaseConfiguration<ComicReportDetail>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicReportDetail> builder)
    {
        builder.ToTable("ComicReportDetails", DbSchema.Comic);
        builder.HasOne(typeof(ComicReport)).WithMany().HasForeignKey("ReportId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
