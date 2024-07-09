#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

namespace Mcsg.Common.SeedWork.Dtos;

using Interfaces;

/// <summary>
/// Id data transfer object
/// </summary>
public class IdDto : IEntityId<Guid>
{
    #region -- Implements --

    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    #endregion
}
