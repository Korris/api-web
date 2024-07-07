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
/// Id data transfer object
/// </summary>
public class IdDto : IEntityId<long>
{
    #region -- Implements --

    /// <summary>
    /// Id
    /// </summary>
    [JsonIgnore]
    public long Id { get; set; }

    #endregion

    #region -- Properties --

    /// <summary>
    /// EncryptedId
    /// </summary>
    public virtual string EncryptedId
    {
        get
        {
            return SecurityAes.EncryptText(Id + "");
        }
        set
        {
            Id = Convert.ToInt64(SecurityAes.DecryptText(value));
        }
    }

    #endregion
}
