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

namespace Mcsg.Common.SeedWork.Dtos;

/// <summary>
/// Dictionary data transfer object
/// </summary>
public class DicDto
{
    #region -- Properties --

    /// <summary>
    /// Gets or sets the key
    /// </summary>
    /// <value>
    /// The key
    /// </value>
    public string Key { get; set; } = default!;

    /// <summary>
    /// Gets or sets the value
    /// </summary>
    /// <value>
    /// The value
    /// </value>
    public object Value { get; set; } = default!;

    #endregion
}
