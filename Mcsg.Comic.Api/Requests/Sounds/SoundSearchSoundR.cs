using System.ComponentModel.DataAnnotations;

namespace Mcsg.Comic.Api.Requests;

public class SoundSearchSoundR : SoundBackgroundMediaLoadR
{
    [Required]
    public string? Keyword { get; set; }
}
