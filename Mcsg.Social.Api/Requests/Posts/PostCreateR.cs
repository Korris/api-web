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

namespace Mcsg.Social.Api.Requests;

/// <summary>
/// Request
/// </summary>
public class PostCreateR : PostFormBase
{
    #region -- Overrides --

    /// <summary>
    /// EncryptedId
    /// </summary>
    [JsonIgnore]
    [SwaggerSchema(ReadOnly = true)]
    public override Guid Id { get => base.Id; set => base.Id = value; }

    #endregion
}
