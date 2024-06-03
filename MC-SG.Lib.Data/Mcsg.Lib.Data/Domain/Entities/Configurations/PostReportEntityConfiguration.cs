using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class PostReportEntityConfiguration : BaseEntityConfiguration<PostReport>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<PostReport> builder)
        {
            builder.ToTable("PostReports");
            builder.HasOne(typeof(Post)).WithMany().HasForeignKey("PostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
        }
    }
}
