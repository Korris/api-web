namespace Mcsg.Api.Areas.TapShow.Models;

/// <summary>
/// Character of a TapShow post
/// </summary>
public class CharacterResponse
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string? Name { get; set; }
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// hashId of the attached avatar (owner only, used to keep the avatar on update); null otherwise
    /// </summary>
    public string? AvatarHashId { get; set; }

    public int Order { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}
