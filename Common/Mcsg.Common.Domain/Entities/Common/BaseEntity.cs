using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities.Common;

using SeedWork.Interfaces;

public abstract class BaseEntity : IEntityId<Guid>
{
    [Key]
    public virtual Guid Id { get; set; }

    public BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}