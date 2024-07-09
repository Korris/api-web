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

namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// Payload data transfer object
/// </summary>
public class PayloadDto
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PayloadDto()
    {
        Roles = [];
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// UserName
    /// </summary>
    public string UserName { get; set; } = default!;

    /// <summary>
    /// ProfileName
    /// </summary>
    public string ProfileName { get; set; } = default!;

    /// <summary>
    /// 0 Guest, 2 Free, 3 Premium
    /// </summary>
    public byte Type { get; set; }

    /// <summary>
    /// Roles
    /// </summary>
    public IList<string> Roles { get; set; }

    /// <summary>
    /// SessionId
    /// </summary>
    [JsonIgnore]
    public Guid SessionId { get; set; }

    #endregion
}
