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

namespace Mcsg.Common.Core.Dtos;

using SeedWork;

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
        UserName = string.Empty;
        Email = string.Empty;
        Roles = [];
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Id
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// EnterpriseId
    /// </summary>
    public ulong? EnterpriseId { get; set; }

    /// <summary>
    /// Encrypted EnterpriseId
    /// </summary>
    public string? EncryptedEnterpriseId
    {
        get
        {
            return (EnterpriseId == null || EnterpriseId == 0) ? "" : SecurityAes.EncryptText(EnterpriseId + "");
        }
        set
        {
            EnterpriseId = string.IsNullOrWhiteSpace(value) ? null : Convert.ToUInt64(SecurityAes.DecryptText(value));
        }
    }

    /// <summary>
    /// UserName
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 1 Individual (for pet owners), 2 Enterprise (for clinic, spa, ...), 3 Administrator
    /// </summary>
    public byte Type { get; set; }

    /// <summary>
    /// Roles
    /// </summary>
    public IList<string> Roles { get; set; }

    #endregion
}
