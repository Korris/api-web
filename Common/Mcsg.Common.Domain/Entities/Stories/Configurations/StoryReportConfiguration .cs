using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Stories.Configurations;

using Core.Constants;

public class StoryReportConfiguration : BaseConfiguration<StoryReport>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryReport> builder)
    {
        builder.ToTable("StoryReports", DbSchema.Story);
    }
}
