using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class SmartCountActionConfiguration : BaseEntityConfiguration<SmartCountAction>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<SmartCountAction> builder)
        {
            builder.ToTable("SmartCountActions");
            builder.Property(x => x.EntityId).IsRequired();
            builder.HasIndex(x => new { x.EntityId, x.ActionType, x.Date }).IsUnique();
        }
    }
}
