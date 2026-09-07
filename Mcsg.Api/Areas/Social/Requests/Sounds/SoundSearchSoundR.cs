using System.ComponentModel.DataAnnotations;

namespace Mcsg.Api.Areas.Social.Requests;

public class SoundSearchSoundR : SoundBackgroundMediaLoadR
{
    [Required]
    public string? Keyword { get; set; }
}
