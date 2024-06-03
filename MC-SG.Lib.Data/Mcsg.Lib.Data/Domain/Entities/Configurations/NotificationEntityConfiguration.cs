using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class NotificationEntityConfiguration : BaseEntityConfiguration<Notification>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");
            builder.Property(x => x.NotificationObjectId).IsRequired();
            builder.HasOne(typeof(NotificationObject)).WithMany().HasForeignKey("NotificationObjectId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("ReceiverId");
        }
    }
}
