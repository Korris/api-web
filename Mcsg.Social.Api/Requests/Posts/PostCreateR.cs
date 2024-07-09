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
