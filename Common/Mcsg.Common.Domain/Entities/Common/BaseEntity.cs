using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities.Common;

using Interface;

public abstract class BaseEntity : IEntity
{
    [Key]
    public virtual Guid Id { get; set; }
    public BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}