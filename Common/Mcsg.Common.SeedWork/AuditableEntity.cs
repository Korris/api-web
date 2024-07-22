namespace Mcsg.Common.SeedWork;

public class AuditableEntity : TrackingEntity
{
    public DateTime? ModifiedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public bool IsDelete { get; set; }

    public AuditableEntity()
    {
        ModifiedDate = DateTime.UtcNow;
    }
}
