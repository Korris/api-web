using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicPostReportConfiguration : BaseConfiguration<ComicPostReport>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicPostReport> builder)
    {
        builder.ToTable("ComicPostReports", DbSchema.Comic);
        builder.HasOne(typeof(ComicPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
