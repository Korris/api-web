using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations
{
    public class ViewHistoryConfiguration : BaseConfiguration<ViewHistory>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<ViewHistory> builder)
        {
            builder.ToTable("ViewHistories");
            builder.Property(x => x.EntityId).IsRequired();
            builder.HasIndex(x => new { x.EntityId, x.UsedId, x.EntityType, x.CreatedDate }).IsUnique();
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
        }
    }
}
