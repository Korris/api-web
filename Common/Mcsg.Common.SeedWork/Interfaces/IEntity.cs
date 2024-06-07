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

namespace Mcsg.Common.SeedWork.Interfaces;

/// <summary>
/// Marker interface to represent an entity
/// </summary>
public interface IEntity
{
    #region -- Properties --

    /// <summary>
    /// Status
    /// </summary>
    byte Status { get; set; }

    /// <summary>
    /// Created by
    /// </summary>
    string CreatedBy { get; set; }

    /// <summary>
    /// Created on
    /// </summary>
    DateTime CreatedOn { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    string? Description { get; set; }

    /// <summary>
    /// Modified by
    /// </summary>
    string? ModifiedBy { get; set; }

    /// <summary>
    /// Modified on
    /// </summary>
    DateTime? ModifiedOn { get; set; }

    #endregion
}

/// <summary>
/// Marker interface to represent an entity
/// </summary>
/// <typeparam name="T">Type of ID</typeparam>
public interface IEntityId<T>
{
    #region -- Properties --

    /// <summary>
    /// Id
    /// </summary>
    T Id { get; set; }

    #endregion
}

/// <summary>
/// Marker interface to represent an entity
/// </summary>
public interface IEntityDevName
{
    #region -- Properties --

    /// <summary>
    /// Developer name
    /// </summary>
    string DevName { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    string Name { get; set; }

    #endregion
}
