namespace Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Create TapShow post request
/// </summary>
public class TapShowPostCreateR : TapShowPostFormBaseR
{
    /// <summary>
    /// Inline characters created together with the post (optional, max TapShowConfig.MaxCharactersPerPost).
    /// Later changes go through api/tapshow/character (POST / PUT / DELETE), never through PUT tapshow.
    /// </summary>
    public List<CharacterItemR>? Characters { get; set; }
}
