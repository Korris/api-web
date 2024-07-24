using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryPostReportConfiguration : BaseConfiguration<StoryPostReport>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryPostReport> builder)
    {
        builder.ToTable("StoryPostReports", DbSchema.Story);
        builder.HasOne(typeof(StoryPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
