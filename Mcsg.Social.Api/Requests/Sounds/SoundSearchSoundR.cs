using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.Requests;

public class SoundSearchSoundR : SoundBackgroundMediaLoadR
{
    [Required]
    public string? Keyword { get; set; }
}
