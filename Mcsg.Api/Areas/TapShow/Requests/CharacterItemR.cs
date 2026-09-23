namespace Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// One character sent inline with POST api/tapshow/tapshow (the "characters" array)
/// </summary>
public class CharacterItemR
{
    #region -- Properties --

    /// <summary>
    /// Display name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// hashId returned by POST api/tapshow/file/upload-media; null → no avatar
    /// </summary>
    public string? AvatarHashId { get; set; }

    /// <summary>
    /// Display order; null → position in the array
    /// </summary>
    public int? Order { get; set; }

    #endregion
}
