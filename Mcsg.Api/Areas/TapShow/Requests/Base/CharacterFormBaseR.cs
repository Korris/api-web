namespace Mcsg.Api.Areas.TapShow.Requests;

using Common.Core.Requests;

/// <summary>
/// Shared fields for create / update character (name + avatar)
/// </summary>
public class CharacterFormBaseR : IdBaseR
{
    #region -- Properties --

    /// <summary>
    /// Display name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// hashId returned by POST api/tapshow/file/upload-media; null → no avatar.
    /// On update: same hashId keeps the current avatar, a new one replaces it, null removes it.
    /// </summary>
    public string? AvatarHashId { get; set; }

    /// <summary>
    /// Display order; null on create → appended at the end
    /// </summary>
    public int? Order { get; set; }

    #endregion
}
