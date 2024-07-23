using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

public class PostReportConfiguration : BaseConfiguration<PostReport>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<PostReport> builder)
    {
        builder.ToTable("PostReports");
        builder.HasOne(typeof(SocialPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
