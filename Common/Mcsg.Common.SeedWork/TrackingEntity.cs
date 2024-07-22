namespace Mcsg.Common.SeedWork;

public class TrackingEntity : BaseEntity
{
    public DateTime CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }

    public TrackingEntity()
    {
        CreatedDate = DateTime.UtcNow;
    }
}
