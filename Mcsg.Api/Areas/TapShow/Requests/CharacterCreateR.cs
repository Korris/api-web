namespace Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Create a character under a TapShow post
/// </summary>
public class CharacterCreateR : CharacterFormBaseR
{
    /// <summary>
    /// HashId of the post that owns the character
    /// </summary>
    public string? PostHashId { get; set; }
}
