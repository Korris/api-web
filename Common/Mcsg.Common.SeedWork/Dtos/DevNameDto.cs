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

using System.Text.Json.Serialization;

namespace Mcsg.Common.SeedWork.Dtos;

using Interfaces;

/// <summary>
/// DevName
/// </summary>
public class DevNameDto : IdDto, IEntityDevName
{
    #region -- Implements --

    /// <summary>
    /// Code (auto generate or manual)
    /// </summary>
    [JsonIgnore]
    public virtual string DevName { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public DevNameDto()
    {
        DevName = string.Empty;
        Name = string.Empty;
    }

    #endregion
}
