using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.SeedWork;

using SeedWork.Interfaces;

public abstract class BaseEntity : IEntityId<Guid>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual Guid Id { get; set; }

    public BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}