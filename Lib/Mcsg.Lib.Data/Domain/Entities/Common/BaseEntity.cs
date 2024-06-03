using Mcsg.Lib.Data.Domain.Entities.Interface;
using System.ComponentModel.DataAnnotations;

namespace Mcsg.Lib.Data.Domain.Entities.Common
{
    public abstract class BaseEntity : IEntity
    {
        [Key]
        public virtual Guid Id { get; set; }
        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}