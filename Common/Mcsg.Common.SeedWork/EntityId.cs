using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.SeedWork;

using SeedWork.Interfaces;

/// <summary>
/// 
/// </summary>
public abstract class EntityId : IEntityId<Guid>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public EntityId()
    {
        Id = Guid.NewGuid();
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual Guid Id { get; set; }

    #endregion
}