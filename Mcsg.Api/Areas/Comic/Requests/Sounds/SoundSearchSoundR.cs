using System.ComponentModel.DataAnnotations;

namespace Mcsg.Api.Areas.Comic.Requests;

public class SoundSearchSoundR : SoundBackgroundMediaLoadR
{
    [Required]
    public string? Keyword { get; set; }
}
