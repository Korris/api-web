using System.ComponentModel.DataAnnotations;

namespace Mcsg.Story.Api.Requests;

public class SoundSearchSoundR : SoundBackgroundMediaLoadR
{
    [Required]
    public string? Keyword { get; set; }
}
