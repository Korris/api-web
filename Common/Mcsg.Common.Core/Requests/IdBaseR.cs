#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace Mcsg.Common.Core.Requests;

using SeedWork.Interfaces;

/// <summary>
/// IdBase request
/// </summary>
public abstract class IdBaseR : BaseR, IEntityId<Guid>
{
    #region -- Implements --

    /// <summary>
    /// Id
    /// </summary>
    [JsonIgnore]
    [SwaggerSchema(ReadOnly = true)]
    public Guid Id { get; set; }

    #endregion
}
