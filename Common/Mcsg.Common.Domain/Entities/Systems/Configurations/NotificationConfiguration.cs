using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class NotificationConfiguration : BaseConfiguration<Notification>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications", DbSchema.System);
        builder.Property(x => x.NotificationObjectId).IsRequired();
        builder.HasOne(typeof(NotificationObject)).WithMany().HasForeignKey("NotificationObjectId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("ReceiverId");
    }
}
