using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations
{
    public class SmartCountActionConfiguration : BaseConfiguration<SmartCountAction>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<SmartCountAction> builder)
        {
            builder.ToTable("SmartCountActions");
            builder.Property(x => x.EntityId).IsRequired();
            builder.HasIndex(x => new { x.EntityId, x.ActionType, x.Date }).IsUnique();
        }
    }
}
