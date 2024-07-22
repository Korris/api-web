namespace Mcsg.Common.Domain.Entities;

using Interfaces;

public class AuditableEntity : BaseEntity, IAuditableEntity
{
    public DateTime CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public bool IsDelete { get; set; }

    public AuditableEntity() : base()
    {
        var now = DateTime.UtcNow;
        CreatedDate = now;
        LastModifiedDate = now;
        IsDelete = false;
    }
}